﻿using core.Modules.User;
using System;
using System.Data;
using System.Data.SqlClient;

namespace core.Modules.Problem
{
    public class ProblemDao : DataAccessObject<ProblemData>
    {
        public ProblemDao() : base("Problem") { }

        /// <summary>
        /// Add a new problem to the database.</summary>
        /// <param name="problem">The ProblemData object with the problem's information</param>
        /// <returns>
        /// true if the add was successful, false otherwise</returns>
        public bool Add(ProblemData problem)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string cmdStr = "Insert into dbo.[" + tableName + "] (";
                string paramList = ") values (";

                SqlCommand cmd = conn.CreateCommand();

                bool insertingName = false;
                //Name
                if (!String.IsNullOrWhiteSpace(problem.Name))
                {
                    insertingName = true;
                    cmdStr += "Name";
                    paramList += "@name";
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar);
                    cmd.Parameters["@name"].Value = problem.Name;
                }

                //Description
                if (!String.IsNullOrWhiteSpace(problem.Description))
                {
                    if (insertingName)
                    {
                        cmdStr += ", ";
                        paramList += ", ";
                    }
                    cmdStr += "Description";
                    paramList += "@description";
                    cmd.Parameters.Add("@description", SqlDbType.NVarChar);
                    cmd.Parameters["@description"].Value = problem.Description;
                }

                cmd.CommandText = cmdStr + paramList + ");";

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Record a user's first attempt at a problem.</summary>
        /// <param name="user">The UserData object with the user's information</param>
        /// <param name="problem">The ProblemData object with the problem's id</param>
        /// <param name="correct">Whether or not the solution is correct</param>
        /// <returns>
        /// true if the operation was successful, false otherwise</returns>
        public bool AddSolution(UserData user, ProblemData problem, bool correct)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "Insert into dbo.[Solution] (UserId, ProblemId, IsCorrect) values (@userId, @problemId, @correct);";

                //User
                cmd.Parameters.Add("@userId", SqlDbType.Int);
                cmd.Parameters["@userId"].Value = user.Id;

                //Problem
                cmd.Parameters.Add("@problemId", SqlDbType.Int);
                cmd.Parameters["@problemId"].Value = problem.Id;

                //Correct
                cmd.Parameters.Add("@correct", SqlDbType.Bit);
                cmd.Parameters["@correct"].Value = correct;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Record a user's attempt at a problem that they previously attempted.</summary>
        /// <param name="user">The UserData object with the user's information</param>
        /// <param name="problem">The ProblemData object with the problem's id</param>
        /// <param name="correct">Whether or not the solution is correct</param>
        /// <returns>
        /// true if the operation was successful, false otherwise</returns>
        public bool UpdateSolution(UserData user, ProblemData problem, bool correct)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "Update dbo.[Solution] Set NumAttempts = NumAttempts + 1, IsCorrect = @correct"
                    + " Where UserId = @userId and ProblemId = @problemId;";

                //User
                cmd.Parameters.Add("@userId", SqlDbType.Int);
                cmd.Parameters["@userId"].Value = user.Id;

                //Problem
                cmd.Parameters.Add("@problemId", SqlDbType.Int);
                cmd.Parameters["@problemId"].Value = problem.Id;

                //Correct
                cmd.Parameters.Add("@correct", SqlDbType.Bit);
                cmd.Parameters["@correct"].Value = correct;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// This method only exists so the superclass DataAccessObject can polymorphically call createFromReader.
        /// Use of the static createFromReader(SqlDataReader) method is preferred.
        /// </summary>
        public override ProblemData createObjectFromReader(SqlDataReader reader)
        {
            return ProblemDao.createFromReader(reader);
        }

        /// <summary>
        /// Creates a problem from a SqlDataReader.</summary>
        /// <param name="reader">The SqlDataReader to get problem data from</param>
        /// <returns>
        /// A ProblemData object</returns>
        public static ProblemData createFromReader(SqlDataReader reader)
        {
            ProblemData problem = new ProblemData((int)reader["Id"]);
            problem.Name = reader["Name"] as string;
            problem.Description = reader["Description"] as string;
            return problem;
        }
    }
}