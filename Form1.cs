using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Calculator
{
    public partial class Form1 : Form
    {
        private string connectionString = "Data Source=DESKTOP-0G8CVOD\\SQLEXPRESS;Initial Catalog=CalculatorDB;Integrated Security=True";
        private SqlConnection connection;

        // Variables to store operand and operator
        private double operand1 = 0;
        private string operation = "";

        public Form1()
        {
            InitializeComponent();
            connection = new SqlConnection(connectionString);
        }

        private void NumberButtonClick(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (textBox_Result.Text == "0" && button.Text != ".")
                textBox_Result.Clear();

            textBox_Result.Text += button.Text;
        }

        private void OperatorClick(object sender, EventArgs e)
        {
            Button button = sender as Button;
            operation = button.Text;
            operand1 = double.Parse(textBox_Result.Text);
            label_Show_Op.Text = $"{operand1} {operation}";
            textBox_Result.Clear();
        }

        private void ClearButtonClick(object sender, EventArgs e)
        {
            textBox_Result.Text = "0";
            label_Show_Op.Text = "";
            operand1 = 0;
            operation = "";
        }

        private void ButtonEquals_Click(object sender, EventArgs e)
        {
            double operand2 = 0;
            if (!double.TryParse(textBox_Result.Text, out operand2))
            {
                MessageBox.Show("Invalid input. Please enter a valid number.");
                return;
            }

            double result = 0;
            string calculation = string.Empty;
            switch (operation)
            {
                case "+":
                    result = operand1 + operand2;
                    calculation = $"{operand1} + {operand2} = {result}";
                    InsertRecord("Addition_table", operand1, operand2, result);
                    break;
                case "-":
                    result = operand1 - operand2;
                    calculation = $"{operand1} - {operand2} = {result}";
                    InsertRecord("Subtraction_table", operand1, operand2, result);
                    break;
                case "*":
                    result = operand1 * operand2;
                    calculation = $"{operand1} * {operand2} = {result}";
                    InsertRecord("Multiplication_table", operand1, operand2, result);
                    break;
                case "/":
                    if (operand2 == 0)
                    {
                        MessageBox.Show("Division by zero is not allowed.");
                        return;
                    }
                    result = operand1 / operand2;
                    calculation = $"{operand1} / {operand2} = {result}";
                    InsertRecord("Division_table", operand1, operand2, result);
                    break;
            }
            textBox_Result.Text = result.ToString();
            label_Show_Op.Text = calculation; // Display the calculation
        }


        private void SquareButtonClick(object sender, EventArgs e)
        {
            double number = double.Parse(textBox_Result.Text);
            double result = number * number;
            string calculation = $"{number}^2 = {result}"; // Calculate and form the calculation string
            textBox_Result.Text = result.ToString();
            label_Show_Op.Text = calculation; // Display the calculation
            InsertRecord("Square_table", number, 0, result);
        }

        private void SquareRootButtonClick(object sender, EventArgs e)
        {
            double number = double.Parse(textBox_Result.Text);
            double result = Math.Sqrt(number);
            string calculation = $"√{number} = {result}"; // Calculate and form the calculation string
            textBox_Result.Text = result.ToString();
            label_Show_Op.Text = calculation; // Display the calculation
            InsertRecord("SquareRoot_table", number, 0, result);
        }


        private void InsertRecord(string tableName, double num1, double num2, double result)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query;
                if (tableName == "Square_table")
                {
                    query = $"INSERT INTO {tableName} (Operand, Result) VALUES (@Num1, @Result)";
                }
                else if (tableName == "SquareRoot_table")
                {
                    query = $"INSERT INTO {tableName} (Operand, Result) VALUES (@Num1, @Result)";
                }
                else
                {
                    query = $"INSERT INTO {tableName} (Operand1, Operand2, Result) VALUES (@Num1, @Num2, @Result)";
                }

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Num1", num1);
                command.Parameters.AddWithValue("@Num2", num2);
                command.Parameters.AddWithValue("@Result", result);

                // Adjust parameters if inserting into SquareRoot_table
                if (tableName == "SquareRoot_table")
                {
                    command.Parameters["@Num2"].Value = DBNull.Value; // Nullify the second operand for square root operation
                    command.Parameters["@Result"].Value = result; // No need to nullify result for square root operation
                }

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

    }
}
