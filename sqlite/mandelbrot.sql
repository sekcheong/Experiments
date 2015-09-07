/*Outlandish Recursive Query Examples

The following query computes an approximation of the Mandelbrot Set and outputs the result as ASCII-art:
*/
    WITH RECURSIVE
      xaxis(x) AS (VALUES(-2.0) UNION ALL SELECT x+0.05 FROM xaxis WHERE x<1.2),
      yaxis(y) AS (VALUES(-1.0) UNION ALL SELECT y+0.1 FROM yaxis WHERE y<1.0),
      m(iter, cx, cy, x, y) AS (
        SELECT 0, x, y, 0.0, 0.0 FROM xaxis, yaxis
        UNION ALL
        SELECT iter+1, cx, cy, x*x-y*y + cx, 2.0*x*y + cy FROM m 
         WHERE (x*x + y*y) < 4.0 AND iter<28
      ),
      m2(iter, cx, cy) AS (
        SELECT max(iter), cx, cy FROM m GROUP BY cx, cy
      ),
      a(t) AS (
        SELECT group_concat( substr(' .+*#', 1+min(iter/7,4), 1), '') 
        FROM m2 GROUP BY cy
      )
    SELECT group_concat(rtrim(t),x'0a') FROM a;

/*
In this query, the "xaxis" and "yaxis" CTEs define the grid of points for which the 
Mandelbrot Set will be approximated. Each row in the "m(iter,cx,cy,x,y)" CTE means 
that after "iter" iterations, the Mandelbrot iteration starting at cx,cy has reached 
point x,y. The number of iterations in this example is limited to 28 (which severely 
limits the resolution of the computation, but is sufficient for low-resolution ASCII-art output). 
The "m2(iter,cx,cy)" CTE holds the maximum number of iterations reached when starting 
at point cx,cy. Finally, each row in the "a(t)" CTE holds a string which is a single 
line of the output ASCII-art. The SELECT statement at the end just queries the "a" CTE 
to retrieve all lines of ASCII-art, one by one.

Running the query above in an SQLite command-line shell results in the following output:

                                        ....#
                                       ..#*..
                                     ..+####+.
                                .......+####....   +
                               ..##+*##########+.++++
                              .+.##################+.
                  .............+###################+.+
                  ..++..#.....*#####################+.
                 ...+#######++#######################.
              ....+*################################.
     #############################################...
              ....+*################################.
                 ...+#######++#######################.
                  ..++..#.....*#####################+.
                  .............+###################+.+
                              .+.##################+.
                               ..##+*##########+.++++
                                .......+####....   +
                                     ..+####+.
                                       ..#*..
                                        ....#
                                        +.

*/