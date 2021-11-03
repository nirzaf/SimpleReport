using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SimpleReport
{
    public partial class Form1 : Form
    {
        private List<Employee> empList = new();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < 20; i++)
            {
                var emp = new Employee
                {
                    EmployeeName = Faker.Internet.UserName(),
                    Address = Faker.Address.StreetAddress(),
                    ContactNumber = Faker.Identification.UkPassportNumber(),
                    Email = Faker.Internet.Email(),
                    CompanyName = Faker.Company.Name()
                };
                empList.Add(emp);
            }

            DGVEmployee.DataSource = empList;
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }
    }
}