using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static System.Resources.ResXFileRef;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace TodoList
{
    public partial class Form1 : Form
    {
        private SQLiteConnection connection;
        private SQLiteCommand command;
        private SQLiteDataAdapter adapter;
        private readonly string connectionString = @"Data Source = todo.db; Version=3;";

        public Form1()
        {
            InitializeComponent();
            CreateTodoTable();
            LoadTodos();

        }

        private void CreateTodoTable()
        {
            connection = new SQLiteConnection(connectionString);
            string query = "CREATE TABLE IF NOT EXISTS Todos (Id INTEGER PRIMARY KEY AUTOINCREMENT, Description TEXT NOT NULL, IsCompleted INTEGER NOT NULL);";
            command = new SQLiteCommand(query, connection);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }


        private void LoadTodos()
        {
            connection = new SQLiteConnection(connectionString);
            string query = "SELECT * FROM Todos;";
            adapter = new SQLiteDataAdapter(query, connection);

            try
            {
                connection.Open();
                var dataTable = new System.Data.DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        


        private void button1_Click(object sender, EventArgs e)
        {
            connection = new SQLiteConnection(connectionString);
            string query = "INSERT INTO Todos (Description, IsCompleted) VALUES (@Description, @IsCompleted);";
            command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@Description", txtDescription.Text);
            command.Parameters.AddWithValue("@IsCompleted", 0);

            try
            {
                connection.Open();
                int result = command.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Todo added successfully.");
                    LoadTodos();
                    txtDescription.Text = "";
                }
                else
                {
                    MessageBox.Show("Todo was not added.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }

        }



        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int rowIndex = dataGridView1.SelectedRows[0].Index;
                int todoId = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells[0].Value);

                connection = new SQLiteConnection(connectionString);
                string query = "UPDATE Todos SET Description=@Description, IsCompleted=@IsCompleted WHERE Id=@Id;";
                command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@Description", txtEdit.Text);
                command.Parameters.AddWithValue("@IsCompleted", chkIsCompleted.Checked ? 1 : 0);
                command.Parameters.AddWithValue("@Id", todoId);
                
                try
                {
                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    

                    if (result > 0)
                    {
                        MessageBox.Show("Todo updated successfully.");
                        LoadTodos();
                        txtEdit.Text = "";
                        chkIsCompleted.Checked = false;
                    }
                    else
                    {
                        MessageBox.Show("Todo was not updated.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close(); 
                }
            }
        }



        private void btnDeleteTodo_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                connection = new SQLiteConnection(connectionString);
                string query = "DELETE FROM Todos WHERE Id=@Id;";
                command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@Id", Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value));

                try
                {
                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Todo deleted successfully.");
                        LoadTodos();
                    }
                    else
                    {
                        MessageBox.Show("Todo was not deleted.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("Please select a todo to delete.");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtEdit.Text = row.Cells["Description"].Value.ToString();
                chkIsCompleted.Checked = Convert.ToBoolean(row.Cells["IsCompleted"].Value);
            }
        }

        private void dataGridView1_CellEndEdit_1(object sender, DataGridViewCellEventArgs e)
        {
            connection = new SQLiteConnection(connectionString);
            string query = "UPDATE Todos SET Description=@Description, IsCompleted=@IsCompleted WHERE Id=@Id;";
            command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@Description", dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
            command.Parameters.AddWithValue("@IsCompleted", Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[2].Value));
            command.Parameters.AddWithValue("@Id", Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value));
            txtEdit.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            try
            {
                connection.Open();
                int result = command.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Todo updated successfully.");
                }
                else
                {
                    MessageBox.Show("Todo was not updated.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }

        }
    }
}
