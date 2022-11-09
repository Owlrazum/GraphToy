public class Draft
{ 

    // private RopesJoin ComputeRopesJoinCanditate()
    // {
    //     Polyline r1, r2;
    //     if (_ropes.Count > 2)
    //     {
    //         int rndIndex = Random.Range(2, _ropes.Count);
    //         r1 = _ropes[rndIndex];
    //         _ropes[rndIndex] = _ropes[0];
    //         _ropes[0] = r1;

    //         rndIndex = Random.Range(2, _ropes.Count);
    //         r2 = _ropes[rndIndex];
    //         _ropes[rndIndex] = _ropes[1];
    //         _ropes[1] = r2;
    //     }
    //     else
    //     {
    //         r1 = _ropes[0];
    //         r2 = _ropes[1];
    //     }

    //     if (r1.ClockOrder == r2.ClockOrder)
    //     {
    //         if (IsNodeHelpTrapPlayer(r2.Start, r1))
    //         {
    //             if (!IsTooSmallDistanceFromPlayer(r1.End, r2.Start))
    //             {
    //                 //return new RopesJoin((r1.end, r2.start));
    //             }
    //         }

    //         if (IsNodeHelpTrapPlayer(r1.Start, r2))
    //         {
    //             if (!IsTooSmallDistanceFromPlayer(r1.Start, r2.End))
    //             {
    //                 //return (r1.start, r2.end);
    //             }
    //         }
    //     }
    //     else
    //     {

    //     }
    //     return new RopesJoin();
    // }
}