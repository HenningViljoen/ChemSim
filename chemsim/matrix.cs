using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chemsim
{
    [Serializable]
    public class matrix
    {
        public List<List<double>> m;

        public matrix(int r, int c, double initialvalue = 0.0) //amount of rows and columns.
        {
            initmatrix(r, c, initialvalue);

            //nullthematrix();
        }

        public matrix() //Constructor for populating the matrix by the user himself.
        {
            m = new List<List<double>>();
        }

        public matrix(matrix matrixcopyfrom)
        {
            copyfrom(matrixcopyfrom);
        }

        public void initmatrix(int r, int c, double initialvalue = 0.0)
        {
            m = new List<List<double>>();
            for (int i = 0; i < r; i++)
            {
                m.Add(new List<double>());
                for (int j = 0; j < c; j++)
                {
                    m[i].Add(initialvalue);
                }
            }
        }

        public void copyfrom(matrix matrixcopyfrom)
        {
            

            int copyfromr = matrixcopyfrom.m.Count();
            int copyfromc;
            if (copyfromr == 0) { copyfromc = 0; }
            else { copyfromc = matrixcopyfrom.m[0].Count(); }

            initmatrix(copyfromr, copyfromc);


            for (int i = 0; i < copyfromr; i++)
            {
                for (int j = 0; j < copyfromc; j++)
                {
                    m[i][j] = matrixcopyfrom.m[i][j];
                }
            }
        }

        public void nullthematrix()
        {
            for (int r = 0; r < m.Count; r++)
            {
                for (int c = 0; c < m[r].Count; c++)
                {
                    m[r][c] = 0;
                }
            }
        }

        public void swoprowsinthematrix(int r1, int r2)
        {
            double[] temprow = new double[m[r1].Count];
            for (int c = 0; c < m[r1].Count; c++)
            {
                temprow[c] = m[r1][c];
                m[r1][c] = m[r2][c];
                m[r2][c] = temprow[c];
            }
        }

        public void deletelastrow()
        {
            if (m.Count > 0) { m.RemoveAt(m.Count - 1); }
        }

        public void addarow()
        {
            if (m.Count > 0)
            {
                m.Add(new List<double>());
                for (int c = 0; c < m[0].Count; c++)
                {
                    m[m.Count - 1].Add(0.0);
                }
            }
        }

        public void deletelastcolumn()
        {
            if (m[0].Count > 0)
            {
                for (int r = 0; r < m.Count; r++)
                {
                    m[r].RemoveAt(m[r].Count - 1);
                }
            }
        }

        public void addacolumn()
        {
            if (m.Count > 0)
            {
                for (int r = 0; r < m.Count; r++)
                {
                    m[r].Add(0.0);
                }
            }
        }

        public void resize(int r, int c)
        {
            while (m.Count > r)
            {
                deletelastrow();
            }
            while (m.Count < r)
            {
                addarow();
            }
            while (m[0].Count > c)
            {
                deletelastcolumn();
            }
            while (m[0].Count < c)
            {
                addacolumn();
            }

        }

        public void idthematrix()
        {
            nullthematrix();
            for (int i = 0; i < m.Count; i++)
            {
                if (i < m[0].Count)
                {
                    m[i][i] = 1;
                }
            }
        }

        public bool issquare()
        {
            return (m.Count == m[0].Count);
        }


        public static matrix operator +(matrix m1, matrix m2)
        {
            int mr = m1.m.Count;
            int mc = m1.m[0].Count;
            matrix mat = new matrix(mr, mc);
            for (int r = 0; r < mr; r++)
            {
                for (int c = 0; c < mc; c++)
                {
                    mat.m[r][c] = m1.m[r][c] + m2.m[r][c];
                }
            }
            return mat;
        }

        public static matrix operator -(matrix m1, matrix m2)
        {
            int mr = m1.m.Count;
            int mc = m1.m[0].Count;
            matrix mat = new matrix(mr, mc);
            for (int r = 0; r < mr; r++)
            {
                for (int c = 0; c < mc; c++)
                {
                    mat.m[r][c] = m1.m[r][c] - m2.m[r][c];
                }
            }
            return mat;
        }

        public static matrix operator *(matrix m1, matrix m2)
        {
            int m1r = m1.m.Count;
            int m1c = m1.m[0].Count;

            int m2r = m2.m.Count;
            int m2c = m2.m[0].Count;
            matrix mat = new matrix(m1r, m2c);
            for (int r = 0; r < m1r; r++)
            {
                for (int c = 0; c < m2c; c++)
                {
                    mat.m[r][c] = 0.0;
                    for (int k = 0; k < m1c; k++)
                    {
                        mat.m[r][c] += m1.m[r][k] * m2.m[k][c];
                    }
                }
            }
            return mat;
        }

        public static matrix operator *(double d, matrix m2)
        {
            int m2r = m2.m.Count;
            int m2c = m2.m[0].Count;
            matrix mat = new matrix(m2r, m2c);
            for (int r = 0; r < m2r; r++)
            {
                for (int c = 0; c < m2c; c++)
                {
                    mat.m[r][c] = d*m2.m[r][c];
                }
            }
            return mat;
        }

        public static matrix transposevector(double[] v)
        {
            int size = v.GetLength(0);
            matrix mat = new matrix(1, size);

            for (int r = 0; r < size; r++)
            {
                mat.m[0][r] = v[r];
            }

            return mat;
        }

        public static matrix transpose(matrix m)
        {
            int mr = m.m.Count;
            int mc = m.m[0].Count;

            matrix mat = new matrix(mc, mr);
            for (int r = 0; r < mr; r++)
            {
                for (int c = 0; c < mc; c++)
                {
                    mat.m[c][r] = m.m[r][c];
                }
            }
            return mat;
        }

        public static void choleskyLDLT(matrix a, matrix l, matrix d)
        {
            int ar = a.m.Count;
            int ac = a.m[0].Count;
            int n = ar; //a must be square.
            double sumdl2 = 0;
            double sumdll = 0;

            //d = identitymatrix(n);
            l.initmatrix(n, n);
            d.initmatrix(n, n);
            matrix c = new matrix(n, n);

            for (int j = 0; j < n; j++)
            {
                sumdl2 = 0;
                for (int s = 0; s <= j - 1; s++)
                {
                    sumdl2 += d.m[s][s] * Math.Pow(l.m[j][s], 2);
                }
                c.m[j][j] = a.m[j][j] - sumdl2;
                d.m[j][j] = c.m[j][j];
                for (int i = j; i < n; i++)
                {
                    sumdll = 0;
                    for (int s = 0; s <= j - 1; s++)
                    {
                       sumdll += d.m[s][s] * l.m[i][s]*l.m[j][s];
                    }
                    c.m[i][j] = a.m[i][j] - sumdll;
                    l.m[i][j] = c.m[i][j] / (d.m[j][j] + global.Epsilon);
                }
            }

        }


        public static double euclideannorm(matrix m) //the norm will be for the whole matrix, but will only really be correct if the matrix is a vector.
        {
            int mr = m.m.Count;
            int mc = m.m[0].Count;
            double norm = 0;
            for (int r = 0; r < mr; r++)
            {
                for (int c = 0; c < mc; c++)
                {
                    norm += Math.Pow(m.m[r][c],2);
                }
            }
            norm = Math.Sqrt(norm);
            return norm;
        }


        public static void solveLYequalsB(matrix L, matrix Y, matrix B) //solves for the X vector the equation: LX = B, with L being a LU decomoposition matrix.  
        // Dimensions have to be n x n for L
        {
            if (L.issquare())
            {
                int Lr = L.m.Count;
                int Lc = L.m[0].Count;
                Y.nullthematrix();
                for (int r = 0; r < Lr; r++)
                {
                    double sum = 0;
                    for (int c = 0; c < r; c++)
                    {
                        sum += L.m[r][c] * Y.m[c][0];
                    }
                    if (L.m[r][r] == 0) { Y.m[r][0] = Double.NaN; }
                    else { Y.m[r][0] = (B.m[r][0] - sum) / L.m[r][r]; }
                }
            }
        }

        

        public static void solveUXequalsY(matrix U, matrix X, matrix Y)
        {
            if (U.issquare())
            {
                int Ur = U.m.Count;
                int Uc = U.m[0].Count;
                X.nullthematrix();
                for (int r = Ur - 1; r >= 0; r--)
                {
                    double sum = 0;
                    for (int c = Uc - 1; c > r; c--)
                    {
                        sum += U.m[r][c] * X.m[c][0];
                    }
                    if (U.m[r][r] == 0) { U.m[r][r] = global.Epsilon; }//X.m[r][0] = Double.NaN; }
                    else { X.m[r][0] = (Y.m[r][0] - sum) / U.m[r][r]; }
                }


            }
        }

        public static void solveAXequalsB(matrix A, matrix X, matrix B)
        {
            int size = A.m.Count();
            matrix lmatrix = new matrix(size, size);
            matrix umatrix = new matrix(size, size);
            matrix ymatrix = new matrix(size, 1);

            A.ludecomposition(lmatrix, umatrix);
            //matrix tempm = lmatrix * umatrix;
            matrix.solveLYequalsB(lmatrix, ymatrix, B);
            //matrix tempm2 = lmatrix * ymatrix;
            matrix.solveUXequalsY(umatrix, X, ymatrix);
        }

        public void ludecomposition(matrix l, matrix u) //LU decomposition with the Doolittle algorithm
        {
            int c = m[0].Count;
            if (issquare() && l.issquare() && u.issquare() && l.m.Count == u.m.Count && l.m.Count == c)
            {
                l.idthematrix();
                u.idthematrix();
                for (int i = 0; i < c; i++)
                {
                    for (int j = i; j < c; j++)
                    {
                        u.m[i][j] = m[i][j];

                        for (int k = 0; k <= i; k++)
                        {
                            if (k != i)
                            {
                                u.m[i][j] = u.m[i][j] - l.m[i][k] * u.m[k][j];
                            }
                        }
                        u.m[i][j] /= l.m[i][i] + global.Epsilon;
                    }

                    for (int j = i + 1; j < c; j++)
                    {
                        l.m[j][i] = m[j][i];

                        for (int k = 0; k < i; k++)
                        {
                            if (k != i)
                            {
                                l.m[j][i] = l.m[j][i] - l.m[j][k] * u.m[k][i];
                            }
                        }
                        l.m[j][i] /= u.m[i][i] + global.Epsilon;
                    }
                }
            }


        }

        public static matrix identitymatrix(int size) //Returns identity matrix.
        {
            matrix mat = new matrix(size, size);
            for (int r = 0; r < size; r++)
            {
                mat.m[r][r] = 1; 
            }
            return mat;
        }


    }
}
