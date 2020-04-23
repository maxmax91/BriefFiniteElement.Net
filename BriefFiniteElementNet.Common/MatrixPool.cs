﻿///Copyright http://geekswithblogs.net/
///

using BriefFiniteElementNet.Common;


namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a pool for matrix. For example in calculating stiffness matrix, the B matrix is several times calculated, then using a matrix pool was usefull
    /// </summary>
    public class MatrixPool
    {
        /// <summary>
        /// Total matrix rents from this pool
        /// </summary>
        public int TotalRents;

        /// <summary>
        /// Total matrix returns to this pool
        /// </summary>
        public int TotalReturns;

        public MatrixPool()
        {
            Pool = new ArrayPool<double>();
        }

        public MatrixPool(ArrayPool<double> pool)
        {
            Pool = pool;
        }

        /// <summary>
        /// The actual pool for keeping the corearrays
        /// </summary>
        private ArrayPool<double> Pool;//= new ArrayPool<double>();


        /// <summary>
        /// extracts a matrix from pool or creates a new one and associate with current pool and return it.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Matrix Allocate(int rows, int columns)
        {
            var arr = Pool.Allocate(rows * columns);

            for (var i = 0; i < arr.Length; i++)
                arr[i] = 0.0;

            var buf = new Matrix(rows, columns, ref arr);

            buf.UsePool = true;
            buf.Pool = this;

            TotalRents++;

            return buf;
        }

        /// <summary>
        /// Returns the matrix to the pool
        /// </summary>
        /// <param name="matrices"></param>
        public void Free(params Matrix[] matrices)
        {
            foreach(var mtx in matrices)
            {
                if (mtx.CoreArray == null)
                    continue;

                Pool.Free(mtx.CoreArray);
                mtx.CoreArray = null;
                TotalReturns++;
            }
        }
    }
}
