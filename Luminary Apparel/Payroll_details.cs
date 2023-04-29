﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Luminary_Apparel
{
    public partial class Payroll_details : Form
    {

        SqlConnection connection = new SqlConnection("Data Source=LAPTOP-4BLCHTST\\SQLEXPRESS;Initial Catalog=Luminary_Apparel;Integrated Security=True");


        public int GID;
        private int count;
        private double overtimeHours;
        private string BasicSalary;

        public Payroll_details(int gID1)
        {
            InitializeComponent();

            GID = gID1;

            showdata();

            CountMonthlyAttendance(2023, 4, GID);


        }

        private void guna2TextBox7_TextChanged(object sender, EventArgs e)
        {

        }


        private void showdata()
        {
            string fname = "";
            string lname = "";
            string jname = "";
            string jexp = "";
            string loanstatus = "";
            string maxloan = "";
            double loanpermonth=0;
            string otperhour = "";
            string EPF = "";
            string ETF = "";



            string query = "SELECT Fname, Lname, Photo FROM EmployeeForm WHERE GID = @GID1";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@GID1", GID);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                fname = reader["Fname"].ToString();
                lname = reader["Lname"].ToString();
                byte[] photoBytes = (byte[])reader["Photo"];

                // Convert byte array to image
                MemoryStream memoryStream = new MemoryStream(photoBytes);
                Image image = Image.FromStream(memoryStream);

                // Set the image to the picture box
                guna2PictureBox1.Image = image;
            }

            // Close connection and clean up resources
            reader.Close();
            connection.Close();

//-----------------------------------RETRIVING JOB DETAILS------------------------------------------------

            string query1 = "SELECT JobName, Experience FROM JobDetailsForm WHERE GID = @GID2";
            SqlCommand command1 = new SqlCommand(query1, connection);
            command1.Parameters.AddWithValue("@GID2", GID);

            connection.Open();
            SqlDataReader reader1 = command1.ExecuteReader();

            while (reader1.Read())
            {
                jname = reader1["JobName"].ToString();
                jexp = reader1["Experience"].ToString();
            }

            // Close connection and clean up resources
            reader1.Close();
            connection.Close();


//-------------------------------------COUNT ATTENDANCE--------------------------------------------------


            DateTime currentDate = DateTime.Now;
            int month = currentDate.Month; // the month for which you want to retrieve the attendance count
            int year = currentDate.Year; // the year for which you want to retrieve the attendance count

            string query3 = "SELECT COUNT(*) FROM AttendanceForm WHERE GID = @GID AND MONTH(STime) = @month AND YEAR(STime) = @year";
            SqlCommand command3 = new SqlCommand(query3, connection);
            command3.Parameters.AddWithValue("@month", month);
            command3.Parameters.AddWithValue("@year", year);
            command3.Parameters.AddWithValue("@GID", GID);

            connection.Open();
            count = (int)command3.ExecuteScalar();
            connection.Close();

//-----------------------------------END COUNT ATTENDANCE--------------------------------------------------

//---------------------------------------OVER-TIME--------------------------------------------------------


            string query4 = "SELECT STime, ETime FROM AttendanceForm WHERE GID = @GID";
            SqlCommand command4 = new SqlCommand(query4, connection);
            command4.Parameters.AddWithValue("@GID", 8777);

            // create variables to store overtime hours and working hours
            overtimeHours = 0;
            double workingHours = 240;

            // open database connection and execute query
            connection.Open();
            SqlDataReader reader4 = command4.ExecuteReader();

            // loop through attendance records and calculate overtime hours
            while (reader4.Read())
            {
                DateTime startTime = Convert.ToDateTime(reader4["STime"]);
                DateTime exitTime = Convert.ToDateTime(reader4["ETime"]);
                TimeSpan difference = exitTime.Subtract(startTime);
                double hours = difference.TotalHours;

                // check if hours worked exceed standard working hours (8 hours)
                if (hours > workingHours)
                {
                    overtimeHours += hours - workingHours;
                }
            }

            // close database connection and reader
            reader.Close();
            connection.Close();

            //-----------------------------------END OVER-TIME--------------------------------------          


            //----------------------------------COLLECT BASIC ALARY DETAILS---------------------------

            string query5 = "SELECT BasicSalary, MaxLoanAmount, OTPerHour, EPFRate ,ETFRate FROM SalaryDB WHERE JobType = @jname AND Experience = @exp";
            SqlCommand command5 = new SqlCommand(query5, connection);
            command5.Parameters.AddWithValue("@jname", jname);
            command5.Parameters.AddWithValue("@exp", jexp);

            connection.Open();
            SqlDataReader reader5 = command5.ExecuteReader();

            while (reader5.Read())
            {
               BasicSalary = reader5["BasicSalary"].ToString();
               maxloan = reader5["MaxLoanAmount"].ToString();
               otperhour = reader5["OTPerHour"].ToString();
                EPF = reader5["EPFRate"].ToString();
                ETF = reader5["ETFRate"].ToString();
            }

            // Close connection and clean up resources
            reader5.Close();
            connection.Close();


            //---------------------------------------------------------END---------------------------------------------

            //--------------------------------------------LOAN DB---------------------------------------------------------

            string query6 = "SELECT LoanStatus FROM Loan WHERE GID = @GID";
            SqlCommand command6 = new SqlCommand(query6, connection);
            command6.Parameters.AddWithValue("@GID", GID);

            connection.Open();
            SqlDataReader reader6 = command6.ExecuteReader();

            while (reader6.Read())
            {
                loanstatus = reader6["LoanStatus"].ToString();
            }

            // Close connection and clean up resources
            reader5.Close();
            connection.Close();


            if (loanstatus == "Online")
            {
                double maxloanx = Convert.ToDouble(maxloan);
                loanpermonth = Math.Round(maxloanx,2) / 48; // 4 year package
            }




            string name = fname+" "+lname;

            double othour = Convert.ToDouble(otperhour);
            double otprice = overtimeHours* othour;

            double netEPF = (Convert.ToDouble(BasicSalary) * Convert.ToDouble(EPF))/100;
            double netETF = (Convert.ToDouble(BasicSalary) * Convert.ToDouble(ETF)) / 100;

            //loan deduction
            double ldeduction = (Convert.ToDouble(BasicSalary) - Convert.ToDouble(loanpermonth));

            //total deduction
            double totaldeduction = ( Convert.ToDouble(loanpermonth) + netEPF + netETF);

            //net salary
            double netsalary = (Convert.ToDouble(BasicSalary) - Convert.ToDouble(loanpermonth) - netEPF - netETF);



            guna2TextBox1.Text = GID.ToString();
            guna2TextBox2.Text = name;
            guna2TextBox4.Text = jname;
            guna2TextBox14.Text = count.ToString();
            guna2TextBox15.Text = overtimeHours.ToString();
            guna2TextBox5.Text = BasicSalary.ToString();
            guna2TextBox12.Text = loanpermonth.ToString();
            guna2TextBox6.Text = otprice.ToString();
            guna2TextBox8.Text = netEPF.ToString();
            guna2TextBox9.Text = netETF.ToString();
            guna2TextBox10.Text =ldeduction.ToString();
            guna2TextBox16.Text = totaldeduction.ToString();
            guna2TextBox11.Text = netsalary.ToString();

            //guna2PictureBox1.Image= image;
        }

        private void CountMonthlyAttendance(int year, int month, int gid)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = new DateTime(year, month, daysInMonth);

            string query = "SELECT COUNT(*) AS AttendanceCount FROM AttendanceForm WHERE GID=@gid AND Status='Full - Day' AND STime>=@startDate AND ETime<=@endDate";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@gid", gid);
            command.Parameters.AddWithValue("@startDate", startDate);
            command.Parameters.AddWithValue("@endDate", endDate);

            connection.Open();
            count = (int)command.ExecuteScalar();
            connection.Close();
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }
    }
}
