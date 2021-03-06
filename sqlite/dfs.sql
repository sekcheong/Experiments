/*An ORDER BY clause on the recursive-select can be used to control 
whether the search of a tree is depth-first or breadth-first. 
To illustrate, we will use a variation on the "org" table from 
an example above, without the "height" column, and with some real data inserted: 
*/

CREATE TABLE org(
  name TEXT PRIMARY KEY,
  boss TEXT REFERENCES org
) WITHOUT ROWID;
INSERT INTO org VALUES('Alice',NULL);
INSERT INTO org VALUES('Bob','Alice');
INSERT INTO org VALUES('Cindy','Alice');
INSERT INTO org VALUES('Dave','Bob');
INSERT INTO org VALUES('Emma','Bob');
INSERT INTO org VALUES('Fred','Cindy');
INSERT INTO org VALUES('Gail','Cindy');


/* Here is a query to show the tree structure in a breadth-first pattern: */

WITH RECURSIVE
  under_alice(name,level) AS (
    VALUES('Alice',0)
    UNION ALL
    SELECT org.name, under_alice.level+1
      FROM org JOIN under_alice ON org.boss=under_alice.name
     ORDER BY 2
  )
SELECT substr('..........',1,level*3) || name FROM under_alice;

/*
The "ORDER BY 2" (which means the same as "ORDER BY under_alice.level+1") 
causes higher levels in the organization chart (with smaller "level" values) 
to be processed first, resulting in a breadth-first search. The output is:

    Alice
    ...Bob
    ...Cindy
    ......Dave
    ......Emma
    ......Fred
    ......Gail

But if we change the ORDER BY clause to add the "DESC" modifier, that will 
cause lower levels in the organization (with larger "level" values) to be 
processed first by the recursive-select, resulting in a depth-first search:
*/
    WITH RECURSIVE
      under_alice(name,level) AS (
        VALUES('Alice',0)
        UNION ALL
        SELECT org.name, under_alice.level+1
          FROM org JOIN under_alice ON org.boss=under_alice.name
         ORDER BY 2 DESC
      )
    SELECT substr('..........',1,level*3) || name FROM under_alice;

/*
The output of this revised query is:

    Alice
    ...Bob
    ......Dave
    ......Emma
    ...Cindy
    ......Fred
    ......Gail

When the ORDER BY clause is omitted from the recursive-select, the queue behaves 
as a FIFO, which results in a breadth-first search. 
*/