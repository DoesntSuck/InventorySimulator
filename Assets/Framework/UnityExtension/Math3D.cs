using UnityEngine;
using System;

namespace UnityExtension
{
    public static class Math3D
    {
        /// <summary>
        /// Generates a random vector in 3-dimensional space from a triangular distribution such that it tends towards the given origin
        /// </summary>
        /// <param name="origin">The vector around which to generate random vertices</param>
        /// <param name="maxDistance">The maximum distance a vector can be generated from the origin</param>
        /// <returns>A vector from a triangular distribution that tend towards the origin vertex</returns>
        public static Vector3 RandomVectorFromTriangularDistribution(Vector3 origin, float maxDistance)
        {
            // Random vertex represent the distance to move from the origin
            Vector3 direction = UnityEngine.Random.insideUnitSphere;

            // Random number from triangular distibution that determines the distance to place point away from the origin
            float distance = Math3D.RandomTriangular(0, maxDistance, 0);

            // Random vector within 'maxDistance' range that tends towards the origin
            return origin + (direction * distance);
        }

        // https://en.wikipedia.org/wiki/Triangular_distribution
        // Calculates a random number from a triangular distribution
        public static float RandomTriangular(float min, float max, float mid)
        {
            // Generate float from uniform distribution
            float unifrom = UnityEngine.Random.Range(0.0f, 1.0f);

            // Mid point in the range 0, 1
            float F = (mid - min) / (max - min);

            // If random number from unifrom distribution occurs on lhs of mid point
            if (unifrom <= F)
                return min + Mathf.Sqrt(unifrom * (max - min) * (mid - min));
            else // ...or occurs on rhs of mid point
                return max - Mathf.Sqrt((1 - unifrom) * (max - min) * (max - mid));
        }

        public static Vector3 Circumcentre(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            // Calculate plane for edge AB
            Vector3 planeABNormal = b - a;
            Vector3 planeABPoint = Vector3.Lerp(a, b, 0.5f);

            // Calculate plane for edge AC
            Vector3 planeACNormal = c - a;
            Vector3 planeACPoint = Vector3.Lerp(a, c, 0.5f);

            // Calculate plane for edge BD
            Vector3 planeBDNormal = d - b;
            Vector3 planeBDPoint = Vector3.Lerp(b, d, 0.5f);

            // Calculate plane for edge CD
            Vector3 planeCDNormal = d - c;
            Vector3 planeCDPoint = Vector3.Lerp(c, d, 0.5f);

            // Calculate line that is the plane-plane intersection between AB and AC
            Vector3 linePoint1;
            Vector3 lineDirection1;

            // Taken from: http://wiki.unity3d.com/index.php/3d_Math_functions
            PlanePlaneIntersection(out linePoint1, out lineDirection1, planeABNormal, planeABPoint, planeACNormal, planeACPoint);

            Vector3 linePoint2;
            Vector3 lineDirection2;

            PlanePlaneIntersection(out linePoint2, out lineDirection2, planeBDNormal, planeBDPoint, planeCDNormal, planeCDPoint);

            // Calculate the point that is the plane-line intersection between the above line and CD
            Vector3 intersection;

            // Floating point inaccuracy often causes these two lines to not intersect, in that case get the two closest points on each line 
            // and average them
            // Taken from: http://wiki.unity3d.com/index.php/3d_Math_functions
            if (!LineLineIntersection(out intersection, linePoint1, lineDirection1, linePoint2, lineDirection2))
            {
                Vector3 closestLine1;
                Vector3 closestLine2;

                // Taken from: http://wiki.unity3d.com/index.php/3d_Math_functions
                ClosestPointsOnTwoLines(out closestLine1, out closestLine2, linePoint1, lineDirection1, linePoint2, lineDirection2);

                // Intersection is halfway between the closest two points on lines
                intersection = Vector3.Lerp(closestLine2, closestLine2, 0.5f);
            }

            return intersection;
        }


        /* http://mathforum.org/kb/message.jspa?messageID=4602531
        Assuming the points aren't collinear, they detemine plane. I presume
        what you want is the circumcircle in that plane.Here is an outline of
        the steps you need to do.

        Call your points P, Q, R.
        Let V and W be the vectors PQ = Q - P and 
        PR = R - P respectively.
        Calculate N = V cross W to get a normal to the plane.
        Calculate the mid-points of PQ and PR, call them M1 and M2.

        Let D1 = N cross V and D2 = N cross W. These are vectors in the plane
        and perpendicular to V and W respectively.

        Use M1 and direction vector D1 to write the parametric equation of the
        perpendicular bisector of PQ and use M2 and direction vector D2 for
        the perpendicular bisector of PR (these are the perpendicular
        bisectors lying in the plane of the triangle).

        These two lines intersect in the circumcentre of your triangle, so
        just solve for where they intersect. */

        /// <summary>
        /// Calculates the circumcentre of the triangle created by the three given position vectors. The circumcentre of a triangle is central point 
        /// of the sphere (also known as the circumsphere) that touches all three vectors of the triangle. The circumcentre is calculated by finding 
        /// the point where the perpendicular bisector of each side of the triangle intersect.
        /// </summary>
        /// <param name="a">The first position vector of the triangle</param>
        /// <param name="b">The second position vector of the triangle</param>
        /// <param name="c">The third position vector of the triangle</param>
        /// <returns>The circumcentre of the circumsphere that touches all three of the given triangle vectors</returns>
        public static Vector3 Circumcentre(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 ab = b - a; // Side ab of triangle
            Vector3 ac = c - a; // Side ac of triangle

            // Normal vector of plane created by three triangle vectors
            Vector3 normal = Vector3.Cross(ab, ac);

            Vector3 m1 = Vector3.Lerp(a, b, 0.5f); // Midpoint of line ab
            Vector3 m2 = Vector3.Lerp(a, c, 0.5f); // Midpoint of line ac

            // Vectors perpendicular to lines ab, and ac
            Vector3 d1 = Vector3.Cross(normal, ab);
            Vector3 d2 = Vector3.Cross(normal, ac);

            Vector3 intersection;

            // If there is no intersection, something has gone wrong!
            if (!LineLineIntersection(out intersection, m1, d1, m2, d2))
            {
                Vector3 closetPointLine1;
                Vector3 closestPointLine2;

                if (!ClosestPointsOnTwoLines(out closetPointLine1, out closestPointLine2, m1, d1, m2, d2))
                    throw new ArithmeticException("Lines are parallel");

                intersection = Vector3.Lerp(closetPointLine1, closestPointLine2, 0.5f);   
            }

            return intersection;
        }

        // http://www.mathopenref.com/trianglecircumcircle.html
        //
        //                                 pqr
        // radius = ----------------------------------------------
        //           √(p + q + r)(q + r - p)(r + p - q)(p + q - r)
        public static float Circumradius(Vector3 a, Vector3 b, Vector3 c)
        {
            float ab = (b - a).magnitude; // p
            float ac = (c - a).magnitude; // q
            float bc = (c - b).magnitude; // r

            // pqr
            float divisor = ab * ac * bc;

            // √(p + q + r)(q + r - p)(r + p - q)(p + q - r)
            float dividend = Mathf.Sqrt((ab + ac + bc) * (ac + bc - ab) * (bc + ab - ac) * (ab + ac - bc));

            float radius = divisor / dividend;
            return radius;
        }

        // http://wiki.unity3d.com/index.php/3d_Math_functions
        //Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
        //Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
        //same plane, use ClosestPointsOnTwoLines() instead.
        public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {

            Vector3 lineVec3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            //is coplanar, and not parrallel
            if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                intersection = linePoint1 + (lineVec1 * s);
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
        }

        //Two non-parallel lines which may or may not touch each other have a point on each line which are closest
        //to each other. This function finds those two points. If the lines are not parallel, the function 
        //outputs true, otherwise false.
        public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {

            closestPointLine1 = Vector3.zero;
            closestPointLine2 = Vector3.zero;

            float a = Vector3.Dot(lineVec1, lineVec1);
            float b = Vector3.Dot(lineVec1, lineVec2);
            float e = Vector3.Dot(lineVec2, lineVec2);

            float d = a * e - b * b;

            //lines are not parallel
            if (d != 0.0f)
            {

                Vector3 r = linePoint1 - linePoint2;
                float c = Vector3.Dot(lineVec1, r);
                float f = Vector3.Dot(lineVec2, r);

                float s = (b * f - c * e) / d;
                float t = (a * f - c * b) / d;

                closestPointLine1 = linePoint1 + lineVec1 * s;
                closestPointLine2 = linePoint2 + lineVec2 * t;

                return true;
            }

            else
            {
                return false;
            }
        }

        //Find the line of intersection between two planes.	The planes are defined by a normal and a point on that plane.
        //The outputs are a point on the line and a vector which indicates it's direction. If the planes are not parallel, 
        //the function outputs true, otherwise false.
        public static bool PlanePlaneIntersection(out Vector3 linePoint, out Vector3 lineVec, Vector3 plane1Normal, Vector3 plane1Position, Vector3 plane2Normal, Vector3 plane2Position)
        {

            linePoint = Vector3.zero;
            lineVec = Vector3.zero;

            //We can get the direction of the line of intersection of the two planes by calculating the 
            //cross product of the normals of the two planes. Note that this is just a direction and the line
            //is not fixed in space yet. We need a point for that to go with the line vector.
            lineVec = Vector3.Cross(plane1Normal, plane2Normal);

            //Next is to calculate a point on the line to fix it's position in space. This is done by finding a vector from
            //the plane2 location, moving parallel to it's plane, and intersecting plane1. To prevent rounding
            //errors, this vector also has to be perpendicular to lineDirection. To get this vector, calculate
            //the cross product of the normal of plane2 and the lineDirection.		
            Vector3 ldir = Vector3.Cross(plane2Normal, lineVec);

            float denominator = Vector3.Dot(plane1Normal, ldir);

            //Prevent divide by zero and rounding errors by requiring about 5 degrees angle between the planes.
            if (Mathf.Abs(denominator) > 0.006f)
            {

                Vector3 plane1ToPlane2 = plane1Position - plane2Position;
                float t = Vector3.Dot(plane1Normal, plane1ToPlane2) / denominator;
                linePoint = plane2Position + t * ldir;

                return true;
            }

            //output not valid
            else
            {
                return false;
            }
        }

        //Get the intersection between a line and a plane. 
        //If the line and plane are not parallel, the function outputs true, otherwise false.
        public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
        {

            float length;
            float dotNumerator;
            float dotDenominator;
            Vector3 vector;
            intersection = Vector3.zero;

            //calculate the distance between the linePoint and the line-plane intersection point
            dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
            dotDenominator = Vector3.Dot(lineVec, planeNormal);

            //line and plane are not parallel
            if (dotDenominator != 0.0f)
            {
                length = dotNumerator / dotDenominator;

                //create a vector from the linePoint to the intersection point
                vector = lineVec.normalized * length;

                //get the coordinates of the line-plane intersection point
                intersection = linePoint + vector;

                return true;
            }

            //output not valid
            else
            {
                return false;
            }
        }
    }
}
