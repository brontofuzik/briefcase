:- module(ww_hybrid_agent, [
		init_agent/1,
		restart_agent/0,
		run_agent/2,
		tell_action/1
	]).

:- use_module(library(clpfd)).

:- ensure_loaded('astar/astar.pl'). % A-star search implementation

:- dynamic([
	size/1,
	pit_prob/1,
	init_location/1,
	init_orientation/1,

	action/2,
	agent_location/2,
	agent_orientation_inf/2,

	pit_free_inf/1,
	pit_inf/1,

	wumpus_free_inf/1,
	wumpus_location_inf/1,
	wumpus_dead_inf/0,

	stench/2,
	breeze/2,
	glitter/2,
	scream/2,
	bump/2,

	game_time/1,
	plan/1,

	goal_states/1,
	allowed_states/1
	]).   

%! init_agent(+Settings:list) is det.
%
%  Initialize the agent.
%
%  Settings = [Size, PitProbability, InitialCell, InitOrientation]

init_agent([Size, PitProb, InitCell, InitOrient]) :-
	retractall(size(_)),
	assertz(size(Size)),

	retractall(pit_prob(_)),
	assertz(pit_prob(PitProb)),

	retractall(init_location(_)),
	assertz(init_location(InitCell)),

	retractall(init_orientation(_)),
	assertz(init_orientation(InitOrient)),

	restart_agent.

%! restart_agent is det.
%
%  Restart the agent.

restart_agent :-
	retractall(action(_,_)),

	retractall(agent_location_inf(_,_)),
	retractall(agent_orientation_inf(_,_)),

	retractall(pit_free_inf(_)),
	retractall(pit_inf(_)),

	retractall(wumpus_free_inf(_)),
	retractall(wumpus_location_inf(_)),
	retractall(wumpus_dead_inf),

	retractall(stench(_,_)),
	retractall(breeze(_,_)),
	retractall(glitter(_,_)),
	retractall(scream(_,_)),
	retractall(bump(_,_)),

	retractall(game_time(_)),
	assertz(game_time(0)),
	retractall(plan(_)),
	assertz(plan([])),

	format('~nNew game started!~n~n', []).

%! run_agent(+Percept, -Action) is det.
%
%  Take a Percept and output an Action.

run_agent(Percept, Action) :-
	game_time(T),
	init_location(StartCell),
	agent_location(Current, T),
	agent_orientation(Angle, T),
	make_percept_statement(Percept, T),
	safe_cells(SafeCells),
	unvisited_cells(UnvisitedCells),
	make_plan(T, StartCell, Current, Angle, SafeCells, UnvisitedCells, [Action|PlanRest]),
	retractall(plan(_)),
	assertz(plan(PlanRest)),
	tell_action(Action).

%! tell_action(+Action:atom) is det.
%
%  Add the fact of Action execution to persistence.

tell_action(Action) :-
	retract(game_time(T)),
	assertz(action(Action, T)),
	format('Action: ~s~n', [Action]),
	TN is T+1,
	assertz(game_time(TN)).

%==============================================

%! make_plan(+T, +StartCell, +Current, +Angle, +SafeCells, +UnvisitedCells, -Plan) is det.
%
%  Return a Plan of actions.
%
%  This predicate implements the logic of decision making.
%
%  @tbd Sometimes wummpus_location succeeds, but have_arrow does not.

make_plan(_,_,_,_,_,_,Plan) :-
	plan(Plan),
	Plan \== [],
	!.
make_plan(T, StartCell, Current, Angle, SafeCells, _, Plan) :-
	glitter(yes, T),
	writeln('Gold! Lets get out of here'),
	plan_route(Current, Angle, [StartCell], SafeCells, _, Plan0),
	append([[grab], Plan0, [climb]], Plan), 
	!.
make_plan(T, _, Current, Angle, SafeCells, _, Plan) :-
	wumpus_location(Cell),
	have_arrow(T),
	!,
	writeln('Going to kill the wumpus!'),
	plan_shot(Current, Angle, [Cell], SafeCells, Plan).
make_plan(_, _, Current, Angle, SafeCells, UnvisitedCells, Plan) :-
	intersection(UnvisitedCells, SafeCells, SafeUnvis),
	SafeUnvis \== [],
	plan_route(Current, Angle, SafeUnvis, SafeCells, Path, Plan),
	!,
	last(Path, Cell1), 
	format('Heading to an unvisited safe cell ~w~n', [Cell1]).
make_plan(T, _, Current, Angle, SafeCells, _, Plan) :-
	have_arrow(T),
	stench(yes, _),
	writeln('Trying to kill the wumpus'),
	possible_wumpus(WCells),
	plan_shot(Current, Angle, WCells, SafeCells, Plan),
	!.
make_plan(_, _, Current, Angle, SafeCells, UnvisitedCells, Plan) :-
	writeln('No choice but to take a chance'),
	not_unsafe(NotUnsafe),
	intersection(UnvisitedCells, NotUnsafe, UnvisNotUnsafe),
	plan_route(Current, Angle, UnvisNotUnsafe, SafeCells, Path, Plan),
	Plan \== [],
	!,
	last(Path, Cell), 
	format('Heading to ~w~n', [Cell]).
make_plan(_, StartCell, Current, Angle, SafeCells, _, Plan) :-
	writeln('I am desperate, exiting the cave'),
	plan_route(Current, Angle, [StartCell], SafeCells, _, Plan0),
	append(Plan0, [climb], Plan),
	!.

%! make_percept_statement(+Percept:list, +T:int) is det.
%
%  Add percepts to persistence.

make_percept_statement([Stench, Breeze, Glitter, Scream, Bump], T) :-
	assertz(stench(Stench, T)),
	assertz(breeze(Breeze, T)),
	assertz(glitter(Glitter, T)),
	assertz(scream(Scream, T)),
	assertz(bump(Bump, T)).

%! agent_location(-Cell:list, +T:int) is semidet.
%
%  Return agent's location Cell at time T.

agent_location(Cell, 0) :- init_location(Cell), ! .
agent_location(Cell, T) :- agent_location_inf(Cell, T), ! .
agent_location([X, Y], T) :-
	T > 0,
	TP is T-1,
	agent_location([XP, YP], TP),
	(	action(forward, TP), !,
		agent_orientation(Angle, TP),
		XN is round(XP + cos(Angle * pi / 180.0)),
		YN is round(YP + sin(Angle * pi / 180.0)),
		cell([XN, YN])
	->
		[X, Y] = [XN, YN]
	;
		[X, Y] = [XP, YP]
	),
	%format('agent_location_inf([~d,~d], ~d)~n', [X,Y,T]),
	assertz( agent_location_inf([X,Y], T) ).

%! agent_orientation(-Angle:int, +T:int) is semidet.
%
%  Return agent's orientation Angle at time T

agent_orientation(Angle, 0) :- init_orientation(Angle), ! .
agent_orientation(Angle, T) :- agent_orientation_inf(Angle, T), ! .
agent_orientation(Angle, T) :-
	T > 0,
	TP is T-1,
	action(Action, TP), !, 
	agent_orientation(AngleP, TP),
	(	Action == turnLeft
	->	Angle is (AngleP + 90) mod 360
	;	Action == turnRight
	->	Angle is (AngleP + 270) mod 360
	;	Angle = AngleP
	),
	%format('agent_orientation_inf(~d, ~d)~n', [Angle,T]),
	assertz( agent_orientation_inf(Angle, T) ) .
	

%! cell(+Cell:list) is semidet.
%! cell(-Cell:list) is multi.
%
%  Succeeds if Cell is a cell of size(Size) from persistence.

cell([X,Y]) :-
	size(S),
	between(1, S, X),
	between(1, S, Y).

%! adjacent(+Cell1:list, +Cell2:list) is semidet.
%! adjacent(?Cell1:list, ?Cell2:list) is multi.
%
%  Succeeds if Cell1 is adjacent to Cell2.

adjacent([X1, Y1], [X2, Y2]) :-
	cell([X1, Y1]),
	cell([X2, Y2]),
	(	X1 #= X2,
		(Y2 #= Y1 - 1; Y2 #= Y1 + 1)
	;	Y1 #= Y2,
		(X2 #= X1 + 1; X2 #= X1 - 1)
	).

%! visited(?Cell:list, ?T:int) is semidet.
%
%  Succeeds if agent's location was Cell at time T.
%  If T is not ground, then return the earliest time of visit or fail.

visited(Cell, 0) :- init_location(Cell) .
visited(Cell, T) :- agent_location_inf(Cell, T), ! .
visited(Current, T) :- 
	game_time(T),
	agent_location(Current, T).

%! visited_cells(-VisitedCells:list) is det.
%
%  Return the list of all visited cells by the current time.

 visited_cells(VisitedCells) :-
	findall(
		Cell,
		(	cell(Cell),
			visited(Cell, _)
		),
		VisitedCells
	).
	
%! unvisited_cells(-UnvisitedCells:list) is det.
%
%  Return the list of all unvisited cells by the current time.

unvisited_cells(UnvisitedCells) :-
	findall(
		Cell,
		(	cell(Cell),
			\+ visited(Cell, _)
		),
		UnvisitedCells
	).

%! pit_free(+Cell:list) is semidet.
%
%  Succeeds if Cell is known to be pit-free.
%
%  A cell is pit-free if:
%
%    * this has been inferred already
%    * the cell has been visited
%    * it has an adjacent which is not breezy
%    * wumpus is located there
%
%  @tbd Handle the case when the wumpus has been killed and you could infer the possible location of wumpus.

pit_free(Cell) :- pit_free_inf(Cell), ! .
% pit_free(Cell) :- pit_inf(Cell), !, fail .
pit_free(Cell) :- visited(Cell, _), ! .
pit_free(Cell) :-
	adjacent(Cell, Cell2),
	visited(Cell2, T),
	breeze(no, T), !,
	assertz( pit_free_inf(Cell) ).
pit_free(Cell) :-
	wumpus_location(Cell), !,
	assertz( pit_free_inf(Cell) ).



%! pit(+Cell:list) is semidet.
%
%  Succeeds if Cell is known to have a pit.
%  
%  A cell has a pit if:
%
%    * this has been inferred already
%    * its adjacent cell is breezy
%      and all other adjacent cells are known to be pit-free.

pit(Cell) :- pit_inf(Cell), ! .
% pit(Cell) :- pit_free_inf(Cell), !, fail .
pit(Cell) :- 
	% \+ visited(Cell, _),
	adjacent(Cell, Cell2),
	visited(Cell2, T),
	breeze(yes, T),
	forall(
		(	adjacent(Cell2, Cell3),
			Cell3 \== Cell
		),
		pit_free(Cell3)
	), 
	!,
	assertz( pit_inf(Cell) ).

%! rotation_angle(+From:list, +To:list, ?Angle:int) is semidet.
%
%  Succeeds if the angle from From to To is Angle (degrees).

rotation_angle([X1, Y1], [X2, Y2], Angle) :-
	DX is X2 - X1,
	DY is Y2 - Y1,
	Angle is round(atan2(DY, DX) * (180.0 / pi)) mod 360.

%! have_arrow(+T:int) is semidet.
%
%  Succeeds if the agent has an arrow at time T.
%  
%  True if the agent has not shot by the time T.

have_arrow(0) :- !.
have_arrow(T) :- 
	action(shoot, TF),
	!,
	T =< TF.
have_arrow(_).

%! wumpus_dead(+T:int) is semidet.
%
%  Succeeds if wumpus is dead.
%
%  Wumpus is dead if there was a scream.

wumpus_dead :- wumpus_dead_inf, !.
wumpus_dead :- 
	scream(yes, _), !,
	assertz( wumpus_dead_inf ).

%! wumpus_free(?Cell:list) is semidet.
%
%  Succeeds if Cell is known to be wumpus-free.
%  
%  A cell is wumpus-free if:
%
%    * the wumpus is dead
%    * this has been inferred already
%    * it has been visited
%    * there is a smelly cell which is not adjacent
%    * its adjacent cell is not smelly
%    * agent shot without killing wumpus in the direction of Cell

wumpus_free(_) :- wumpus_dead, ! .
wumpus_free(Cell) :- wumpus_free_inf(Cell), ! .
wumpus_free(Cell) :- visited(Cell, _), ! .
wumpus_free(Cell) :-
	stench(yes, T),
	agent_location(Cell1, T),
	\+ adjacent(Cell, Cell1), 
	!,
	assertz( wumpus_free_inf(Cell) ).
wumpus_free(Cell) :-
	adjacent(Cell, Cell2),
	visited(Cell2, T),
	stench(no, T), !,
	assertz( wumpus_free_inf(Cell) ).
wumpus_free(Cell) :-
	action(shoot, T),
	have_arrow(T),
	agent_location(AgentCell, T),
	agent_orientation(Angle, T),
	rotation_angle(AgentCell, Cell, Angle), !,
	assertz( wumpus_free_inf(Cell) ).

%! wumpus_location(?Cell:list) is semidet.
%
%  Succeeds if wumpus is known to be in Cell.
%  
%  A wumpus is in a cell if:
%
%    * this has been inferred already
%    * its adjacent cell is smelly
%      and all other adjacent cells are known to be wumpus-free.
%    * it has two different adjacent smelly cells, and the symmetric cell is wumpus-free

wumpus_location(_) :-
	wumpus_dead, !, fail.
wumpus_location(Cell) :-
	wumpus_location_inf(Cell1),
	!,
	(	Cell == Cell1
	->	true
	;	fail
	).
wumpus_location(Cell) :-
	adjacent(Cell, Cell2),
	visited(Cell2, T),
	stench(yes, T),
	forall(
		(	adjacent(Cell2, Cell3),
			Cell3 \== Cell
		),
		wumpus_free(Cell3)
	),
	!,
	format('Wumpus is located at ~w~n', [Cell]),
	assertz( wumpus_location_inf(Cell) ).
wumpus_location([X,Y]) :- 
	adjacent([X,Y], [X,Y1]),
	visited([X,Y1], T1),
	stench(yes, T1),
	adjacent([X,Y], [X1,Y]),
	visited([X1,Y], T2),	
	stench(yes, T2),
	wumpus_free([X1,Y1]),
	!,
	format('Wumpus is located at [~w,~w]~n', [X,Y]),
	assertz( wumpus_location_inf([X,Y]) ).

%! possible_wumpus(-Cells:list) is det.
%
%  Return the list of possible wumpus locations.
%  
%  The list consists of:
%    * one cell, if wumpus location can be determined, otherwise
%    * all cells which are not wumpus-free

possible_wumpus([Cell]) :-
	wumpus_location(Cell), !.
possible_wumpus(Cells) :-
	findall(
		Cell,
		(	cell(Cell),
			\+ wumpus_free(Cell)
		),
		Cells
	).

%  safe_cells(-SafeCells:list) is det.
%
%  Find all safe cells.

safe_cells(SafeCells) :-
	findall(
		Cell,
		(	cell(Cell),
			pit_free(Cell),
			wumpus_free(Cell)
		),
		SafeCells
	).

%! not_unsafe(-NotUnsafeCells:list) is det.
%
%  Find all cells that are not unsafe.

not_unsafe(NotUnsafeCells) :-
	findall(
		Cell,
		(	cell(Cell),
			\+ pit(Cell),
			\+ wumpus_location(Cell)
		),
		NotUnsafeCells
	).

%==============================================

%! plan_route(+Current:list, +Angle:int, +Goals:list, +Allowed:list, -Path:list, -Plan:list) is semidet.
%
%  Return the Path and Plan of actions to get from Current
%  oriented at Angle to any of Goals through Allowed cells.

plan_route(_, [], _, []).
plan_route(Current, Angle, Goals, Allowed0, Path, Plan) :-
	append([[Current], Goals, Allowed0], Allowed1),
	list_to_set(Allowed1, Allowed),
	make_route_problem(Goals, Allowed),
	astar(Current, successor, goal_test, heuristic, Path, equal_test),
	path2actions(Path, Angle, Plan).

%! make_route_problem(+Goals:list, +Allowed:list) is det.
%
%  Remember goals and allowed cells.
%
%  This is used to setup auxilary function for A*-search: 
%  goal_test, successor, heuristic and equal_test

make_route_problem(Goals, Allowed) :-
	retractall(goal_states(_)),
	asserta(goal_states(Goals)),
	retractall(allowed_states(_)),
	asserta(allowed_states(Allowed)).

goal_test(Goal) :-
	goal_states(Goals),
	member(Goal, Goals).

successor(Cell, Successors) :-
	allowed_states(Allowed),
	findall(
		(1, Adjacent),
		(	member(Adjacent, Allowed),
			adjacent(Cell, Adjacent)
		),
		Successors
	).

heuristic([X,Y], Hval) :-
	goal_states(Goals),
	findall(
		ManhattanDist,
		(	member([X1,Y1], Goals),
			ManhattanDist is abs(X-X1) + abs(Y-Y1)
		),
		MDistList
	),
	min_list(MDistList, Hval).

equal_test(C,C).

%! path2actions(+Path:list, +AngleNow:int, -Actions:list) is semidet.
%
%  Return a list of Actions to move along the Path
%  with initial orientation AngleNow

path2actions([], _, []).
path2actions([_], _, []).
path2actions([Current,Next|Path], AngleNow, Actions) :-
	rotation_angle(Current, Next, Angle),
	plan_rotation(AngleNow, Angle, Actions0),
	path2actions([Next|Path], Angle, Actions1),
	append([Actions0, [forward], Actions1], Actions), !.

%! plan_rotation(+Angle1:int, +Angle2:int, -Actions:list) is semidet.
%
%  Return a list of Actions to turn from Angle1 to Angle2

plan_rotation(Angle, Angle, []) :- !.
plan_rotation(270, 0, [turnLeft]) :- !.
plan_rotation(0, 270, [turnRight]) :- !.
plan_rotation(Angle1, Angle2, [turnLeft, turnLeft]) :-
	180 is abs(Angle1 - Angle2), !.
plan_rotation(Angle1, Angle2, [turnLeft]) :-
	Angle1 < Angle2, !.
plan_rotation(Angle1, Angle2, [turnRight]) :-
	Angle1 > Angle2, !.

%! grid_of(+Cells:list, -Grid:lest) is det.
%
%  Return the Grid consisting of all cells which are on the same 
%  vertical or horizontal line with any of Cells

grid_of(Cells, Grid) :-
	findall(
		[X, Y],
		(	cell([X, Y]),
			(
				memberchk([X, _], Cells)
			;
				memberchk([_, Y], Cells)
			)
		),
		Grid0
	),
	list_to_set(Grid0, Grid).

%! plan_shot(+Current:list, +Angle:int, +PossibleTargets:list, +Allowed:list, -Plan:list) is semidet.
%
%  Return the Plan of actions to shoot any of PossibleTargets
%  from Current oriented at Angle.

plan_shot(Current, Angle, PossibleTargets, Allowed, Plan) :-
	grid_of(PossibleTargets, Grid1),
	intersection(Grid1, Allowed, Goals),

	plan_route(Current, Angle, Goals, Allowed, Path, Plan0),
	(	Plan0 == []
	->	Angle0 = Angle,
		ShootFrom = Current
	;	reverse(Path, PathRev),
		[ShootFrom, ShootFromPrev|_] = PathRev,
		rotation_angle(ShootFromPrev, ShootFrom, Angle0)
	),
	grid_of([ShootFrom], Grid2),
	intersection(Grid2, PossibleTargets, SomeTargets),
	random_member(Target, SomeTargets),

	rotation_angle(ShootFrom, Target, Angle1),
	plan_rotation(Angle0, Angle1, ActionsTurn),
	append([Plan0, ActionsTurn, [shoot]], Plan), !.