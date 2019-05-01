:- dynamic([ispit/1, iswumpus/1], nopit/1, nowumpus/1).

sensebreeze([X, Y], true) :-
    XEast is X + 1, (nopit([XEast, Y]); assert(ispit([XEast, Y]))), 
    XWest is X - 1, (nopit([XWest, Y]); assert(ispit([XWest, Y]))),
	YNorth is Y + 1, (nopit([X, YNorth]); assert(ispit([X, YNorth]))),
    YSouth is Y - 1, (nopit([X, YSouth]); assert(ispit([X, YSouth]))).

sensebreeze([X, Y], false) :- 
    XEast is X + 1, retractall(ispit([XEast, Y])), assert(nopit([XEast, Y])),
    XWest is X - 1, retractall(ispit([XWest, Y])), assert(nopit([XWest, Y])),
	YNorth is Y + 1, retractall(ispit([X, YNorth])), assert(nopit([X, YNorth])),
    YSouth is Y - 1, retractall(ispit([X, YSouth])), assert(nopit([X, YSouth])).

sensestench([X, Y], true) :- 
    XEast is X + 1, (nowumpus([XEast, Y]); assert(iswumpus([XEast, Y]))),
    XWest is X - 1, (nowumpus([XWest, Y]); assert(iswumpus([XWest, Y]))),
	YNorth is Y + 1, (nowumpus([X, YNorth]); assert(iswumpus([X, YNorth]))),
    YSouth is Y - 1, (nowumpus([X, YSouth]); assert(iswumpus([X, YSouth]))).

sensestench([X, Y], false) :- 
    XEast is X + 1, retractall(iswumpus([XEast, Y])), assert(nowumpus([XEast, Y])),
    XWest is X - 1, retractall(iswumpus([XWest, Y])), assert(nowumpus([XWest, Y])),
	YNorth is Y + 1, retractall(iswumpus([X, YNorth])), assert(nowumpus([X, YNorth]))
    YSouth is Y - 1, retractall(iswumpus([X, YSouth])), assert(nowumpus([X, YSouth])).