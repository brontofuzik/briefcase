/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
   Wumpus World
   Written Jan. 25th 2005 Markus Triska triska@metalevel.at
   Public domain code.
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */


/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  The main control flow is realized via a make/next/done interface.
  The agent is queried for its action in each step. We maintain its state (as
  it perceives it itself), its actual position and some other trivia.  All
  predicates belonging to the control side are prefixed by "c_" to not
  interfere with a user's agent code.


  Example world (P = pit, W = wumpus, G = gold):

  ======================
  |    |     |    | P  |
  |----|-----|----|----|
  | W  |  G  | P  |    |
  |----|-----|----|----|
  |1/2 |     |    |    |
  |----|-----|----|----|
  |1/1 | 2/1 | P  |    |
  ======================

  Sequence of actions:

   ?- c_action(A).
   %@ A = right ;
   %@ A = right ;
   %@ A = forward ;
   %@ A = right ;
   %@ A = right .
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

:- use_module(library(clpfd)).


% Example world shown above.
c_world(W) :-
        W = [1/3-wumpus,2/3-gold,3/3-pit,3/1-pit,4/4-pit].



c_world_pos_ent(Pos, Ent) :-
        c_world(W),
        member(Pos-Ent, W).


c_world_pos_near(Pos, Ent, Perc) :-
        (   c_world_pos_ent(Pos, Ent) ->
            Perc = 1
        ;   c_pos_neighbour(Pos,Nb), c_world_pos_ent(Nb,Ent) ->
            Perc = 1
        ;   Perc = 0
        ).

c_world_pos_perception(Actions, Pos-_Dir, Perc) :-
        c_world_pos_near(Pos, wumpus, Stench),
        c_world_pos_near(Pos, pit, Breeze),
        ((c_world_pos_ent(Pos,gold), \+ member(pa(Pos-_,grab),Actions)) ->
                Glitter = 1 ; Glitter = 0 ),
        Perc = perc(Stench,Breeze,Glitter,_Scream,_Bump).



c_pos_neighbour(X/Y, NX/NY) :-
        (   NX #= X - 1, NY = Y
        ;   NX #= X + 1, NY = Y
        ;   NX = X, NY #= Y - 1
        ;   NX = X, NY #= Y + 1
        ).

c_startpos(1/1-_).
c_startdir(right).


c_agent_faces(X/Y-right, X1/Y) :- X1 #> X.
c_agent_faces(X/Y-down, X/Y1)  :- Y1 #< Y.
c_agent_faces(X/Y-left, X1/Y)  :- X1 #< X.
c_agent_faces(X/Y-up, X/Y1)    :- Y1 #> Y.

c_error(Error) :-
        writeln(Error),
        false.

c_valid_action(Action, _Pos, _Hist) :-
        memberchk(Action, [right,left,forward]),
        !.
c_valid_action(shoot, _Pos, Hist) :-
        (   member(pa(_,shoot), Hist) ->
            c_error('Agent tried to shoot again!')
        ;   true
        ).
c_valid_action(grab, Pos, Hist) :-
        (   c_world_pos_perception(Hist, Pos, perc(_St,_Br,1,_Scr,_Bump)) ->
            true
        ;   c_error('Cannot grab here!')
        ).
c_valid_action(climb, Pos, _Hist) :-
        (   c_startpos(Pos) ->
            true
        ;   c_error('Cannot climb out here.')
        ).

c_no_wumpus(Pos, Hist) :-
        (   c_world_pos_ent(Pos, wumpus) ->
            member(pa(Somepos,shoot), Hist),
            c_agent_faces(Somepos, Pos)
        ;   true
        ).


c_valid_pos(Pos-_Dir,Hist) :-
        (   c_world_pos_ent(Pos, pit) ->
            c_error('Fell into pit - dead.')
        ;   true
        ),
        (   c_no_wumpus(Pos, Hist) ->
            true
        ;   c_error('Ran into wumpus - dead.')
        ).


c_make(S) :-
        c_startpos(Startpos-Startdir),
        c_startdir(Startdir),
        agent_make(S0),
        S = ag(S0)-rp(Startpos-Startdir)-bs(0,0)-act([]).

c_done(S) :-
        S = ag(_Sta)-rp(_Realpos)-bs(_B,_S)-act(Actions),
        member(pa(_Pos,climb), Actions).


c_rotation(right).
c_rotation(left).

c_pos_move_newpos(Pos, climb, Pos).
c_pos_move_newpos(Pos, grab, Pos).
c_pos_move_newpos(Pos, shoot, Pos).
c_pos_move_newpos(X/Y-right, forward, X1/Y-right) :- X1 #= X + 1.
c_pos_move_newpos(X/Y-down, forward, X/Y1-down)   :- Y1 #= Y - 1.
c_pos_move_newpos(X/Y-left, forward, X1/Y-left)   :- X1 #= X - 1.
c_pos_move_newpos(X/Y-up, forward, X/Y1-up)       :- Y1 #= Y + 1.
c_pos_move_newpos(X/Y-Dir, Rot, X/Y-NDir) :-
        c_rotation(Rot),
        c_dir_rot_newdir(Dir, Rot, NDir).

c_dir_rot_newdir(right, right, down).
c_dir_rot_newdir(right, left, up).
c_dir_rot_newdir(down, right, left).
c_dir_rot_newdir(down, left, right).
c_dir_rot_newdir(left, right, up).
c_dir_rot_newdir(left, left, down).
c_dir_rot_newdir(up, right, right).
c_dir_rot_newdir(up, left, left).

/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  Main control predicate, keeping track of the agent's state (what it thinks
  of itself), and its actual position, also whether it bumped, killed a Wumpus
  and a list of its previous actions.

  If the agent bumps into a wall or kills a Wumpus, we need to tell it in the
  next step. Perceptions are forwarded to the agent via a perc/5 term:
  Perception = perc(Stench,Breeze,Glitter,Scream,Bump), where Stench, Breeze
  etc. are either 0 or 1.
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

c_next(ag(_)-rp(_)-bs(_,_)-act([pa(_Pos,Action)|_]), Action).
c_next(S0, done) :-
        c_done(S0).
c_next(S0, S) :-
        \+ c_done(S0),
        S0 = ag(Ag0)-rp(Pos0)-bs(Bumped0,Screamed0)-act(Actions0),
        Perc = perc(_,_,_,Screamed0,Bumped0),
        c_world_pos_perception(Actions0, Pos0, Perc),
        agent_next(Ag0, Perc, Action, Ag),
        % nl, write('agent at: '), write(Pos0),
        ( var(Action) ->
                c_error('Agent does not react.')
        ;
                true
        ),
        c_valid_action(Action, Pos0, Actions0),
        ((Action == shoot,c_world_pos_ent(WuPos,wumpus),c_agent_faces(Pos0,WuPos)) -> Scream = 1 ; Scream = 0),
        ( c_bumping(Pos0, Action) ->
                Bumped = 1,
                Pos = Pos0  % no change of position
        ;
                Bumped = 0,
                c_pos_move_newpos(Pos0, Action, Pos)
        ),
        c_valid_pos(Pos, Actions0),
        c_next(ag(Ag)-rp(Pos)-bs(Bumped,Scream)-act([pa(Pos0,Action)|Actions0]), S).


% currently, use a 4x4-field
c_bumping(Pos,forward) :-
        (Pos = 1/_-left ; Pos = _/1-down ; Pos = 4/_-right ; Pos = _/4-up).


c_action(A) :-
        c_make(S),
        c_next(S, A).

c_actions(As) :-
        findall(A, c_action(A), As).

/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  A simple example agent follows.  To write your own agent, supply
  agent_make/1 and agent_next/4:

  agent_make(-State):
      create initial State of agent

  agent_next(+State0, +Perc, -Action, -State)
      given the agent's state State0 and a perception Perc on the
      current position, compute an Action and a new agent State


  A trivial agent:

  agent_make(S0) :- S0 = [forward,left,forward,forward,grab,left,
                          shoot,left,forward,forward,right,forward,climb].
  agent_next([S|Ss], _, S, Ss).

  It starts with a fixed list of actions and performs the next one in
  each step, ignoring all perceptions.

  ----------------------------------------------------------------------

  This agent starts believing it is in 1/1 and looking right and has neither
  plan nor history initially.

  agent state: s(Currpos-Direction, Plan, Hist)
  history: h(Pos,Plan,Perc,Action)
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

agent_make(S0) :-
        S0 = s(1/1-right,[],[]).


agent_next(S0, Perc, grab, S) :-
        Perc = perc(_Stench,_Breeze,1,_Scream,_Bump),
        !, % Gold? ==> grab
        S0 = s(Pos, Plan, Hist),
        S = s(Pos, Plan, [h(Pos,Plan,Perc,grab)|Hist]).
agent_next(S0, perc(_St,_Br,_Gl,_Scream,1), Action, S) :- !,
        % we bumped - previous position is correct, forget our plan
        S0 = s(_Pos,_Plan,Hist),
        Hist = [h(HPos,_,_,_)|_],
        Action = right, % do anything
        pos_move_newpos(HPos, Action, Newpos),
        S = s(Newpos, [], Hist).
agent_next(s(Pos,[climb],Hist), _Perc, climb, s(Pos,[],Hist)) :- !.
agent_next(s(Pos,[Plan1|Plans],Hist), Perc, Action, S) :- !,
        % normally, we only follow the plan we made previously
        pos_nextpos_goodmove(Pos, Plan1, Action),
        pos_move_newpos(Pos, Action, Pos1-Dir1),
        ( Pos1 = Plan1 ->
                Nextplan = Plans  % reached desired position, head for next
        ;
                Nextplan = [Plan1|Plans]
        ),
        S = s(Pos1-Dir1,Nextplan,[h(Pos,[Plan1|Plans],Perc,Action)|Hist]).
agent_next(s(Pos0,[],Hist0), Perc, Action, S) :-
        % plan is used up - build a new one if possible
        Perc = perc(_St,_Br,0,_Scr,0),
        Hist = [h(Pos0,[],Perc,Action)|Hist0],
        Pos0 = P0-_Dir0,
        ( next_target(Hist, Target) ->
                once(pos_target_hist_plan(P0,Target,Hist,Plan)),
                Plan = [Plan1|_],
                pos_nextpos_goodmove(Pos0, Plan1, Action),
                pos_move_newpos(Pos0, Action, Newpos),
                S = s(Newpos, Plan, Hist)
        ;
                % no new target left - get out of this maze
                once(pos_target_hist_plan(P0,1/1,Hist,Rescueplan0)),
                append(Rescueplan0, [climb], Rescueplan),
                Action = right,  % do anything
                pos_move_newpos(Pos0, Action, Newpos),
                S = s(Newpos,Rescueplan,Hist)
        ).



pos_visited(Pos, Hist) :-
        member(h(Pos-_Dir,_Plan,_Perc,_Action), Hist).

in_previous_plan(Pos, Hist) :-
        member(h(_Pos,Plan,_Perc,_Action), Hist),
        memberchk(Pos, Plan).

pos_perception(Pos, Hist, Perc) :-
        member(h(Pos-_Dir,_Plan,Perc,_Action), Hist).


no_wumpus(Pos, Hist) :-
        pos_neighbour(Pos, Neighbour),
        pos_visited(Neighbour, Hist),
        pos_perception(Neighbour, Hist, perc(0,_Breeze,_Glitter,_Scream,_Bump)).

no_pit(Pos, Hist) :-
        pos_neighbour(Pos, Neighbour),
        pos_visited(Neighbour, Hist),
        pos_perception(Neighbour, Hist, perc(_Stench,0,_Glitter,_Scream,_Bump)).


next_target(Hist,Target) :-
        pos_visited(Somepos, Hist),
        pos_neighbour(Somepos, Neighbour),
        \+ pos_visited(Neighbour, Hist),
        \+ in_previous_plan(Neighbour, Hist),
        no_wumpus(Neighbour, Hist),
        no_pit(Neighbour, Hist),
        Target = Neighbour.


% make a plan about how to go from Pos to Target, moving along nodes that
% were already visited previously

pos_target_hist_plan(Pos, Target, _Hist, [Target]) :-
        pos_neighbour(Pos, Target).
pos_target_hist_plan(Pos, Target, Hist, [Next|Rest]) :-
        pos_neighbour(Pos, Next),
        pos_visited(Next, Hist),
        pos_target_hist_plan(Next, Target, Hist, Rest).


pos_faces(right, X/Y, X1/Y) :- X1 #> X.
pos_faces(down, X/Y, X/Y1)  :- Y1 #< Y.
pos_faces(left, X/Y, X1/Y)  :- X1 #< X.
pos_faces(up, X/Y, X/Y1)    :- Y1 #> Y.

pos_nextpos_goodmove(Coord-Dir, Next, Move) :-
        (   pos_faces(Dir, Coord, Next) ->
            Move = forward
        ;   Move = right
        ).


rotation(right).
rotation(left).

pos_move_newpos(X/Y-right, forward, X1/Y-right) :- X1 #= X + 1.
pos_move_newpos(X/Y-down, forward, X/Y1-down) :-   Y1 #= Y - 1.
pos_move_newpos(X/Y-left, forward, X1/Y-left) :-   X1 #= X - 1.
pos_move_newpos(X/Y-up, forward, X/Y1-up) :-       Y1 #= Y + 1.
pos_move_newpos(X/Y-Dir, Rot, X/Y-NDir) :-
        rotation(Rot),
        dir_rot_newdir(Dir, Rot, NDir).

dir_rot_newdir(right, right, down).
dir_rot_newdir(right, left, up).
dir_rot_newdir(down, right, left).
dir_rot_newdir(down, left, right).
dir_rot_newdir(left, right, up).
dir_rot_newdir(left, left, down).
dir_rot_newdir(up, right, right).
dir_rot_newdir(up, left, left).

pos_neighbour(X/Y, NX/NY) :-
        (   NX #= X - 1, NY = Y
        ;   NX #= X + 1, NY = Y
        ;   NX = X, NY #= Y - 1
        ;   NX = X, NY #= Y + 1
        ).
