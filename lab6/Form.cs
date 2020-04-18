using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Npgsql;
using NpgsqlTypes;
using System.Data.SqlClient; // Sql Server Connection Namespace

namespace lab6
{
    public partial class FormPresentantion : Form
    {
        private readonly string connect_string = "Host = localhost; Username = postgres; password = postgres; Database = lab6";

        public FormPresentantion()
        {
            InitializeComponent();
            InitializePresentation();
        }
        private void InitializePresentation()
        {
            data_grid_view.Rows.Clear();
            data_grid_view.Columns.Clear();
            DataTable data = new DataTable();
            status_load(data);
            {
                data_grid_view.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "id_question",
                    ValueType = typeof(int),
                    Visible = false
                });
                data_grid_view.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "link_id",
                    HeaderText = "link_id",
                    ValueType = typeof(int)
                });
                data_grid_view.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "count_view",
                    HeaderText = "count_view",
                    ValueType = typeof(int),
                });
                data_grid_view.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "count_like",
                    HeaderText = "count_like",
                    ValueType = typeof(int),
                });
                data_grid_view.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "user_id",
                    HeaderText = "id_user",
                    Visible = false,
                    ValueType = typeof(int)
                });
                data_grid_view.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "login",
                    HeaderText = "login",
                    ValueType = typeof(string)
                });
                data_grid_view.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "age",
                    HeaderText = "age",
                    ValueType = typeof(int)
                });
                data_grid_view.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "status_id",
                    HeaderText = "status_id",
                    Visible = false,
                    ValueType = typeof(int)
                });
                data_grid_view.Columns.Add(new DataGridViewComboBoxColumn
                {
                    Name = "status",
                    HeaderText = "status",
                    ValueType = typeof(string),
                    DataSource = data,
                    DisplayMember = "name"
                });
            }
            using (var connect = new NpgsqlConnection(connect_string))
            {
                connect.Open();
                var sCommand = new NpgsqlCommand
                {
                    Connection = connect,
                    CommandText = @"select * from questions inner join users u on questions.user_id = u.id_user inner join statuses s on u.status_id = s.id_status;"
                };
                var reader = sCommand.ExecuteReader();
                while (reader.Read())
                {
                    data_grid_view.Rows.Add(reader["id_question"], reader["link_id"], reader["count_view"], reader["count_like"], reader["user_id"], reader["login"], reader["age"], reader["status_id"],
                        reader["name"]); //fill data_grid_view
                }
            }
            foreach (DataGridViewRow row in data_grid_view.Rows)
            {
                if (!row.IsNewRow)
                {
                    var tag = new List<object>();
                    foreach (DataGridViewCell cell in row.Cells)
                        tag.Add(cell.Value);
                    row.Tag = tag;
                } //assign a tag for each added row equal his values
            }
        }

        private void data_grid_view_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            var row = data_grid_view.Rows[e.RowIndex];

            if (data_grid_view.IsCurrentRowDirty)
            {
                if (!e.Cancel)
                {
                    if (row.Cells["login"].Value == null)
                    {
                        MessageBox.Show("Заполните обязательное поле login", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (row.Cells["status"].Value == null) row.Cells["status"].Value = "Hide";
                    using var connect = new NpgsqlConnection(connect_string);
                    connect.Open();

                    using var command_status = new NpgsqlCommand() { Connection = connect };
                    command_status.CommandText = "select id_status from statuses where name = @name";
                    command_status.Parameters.AddWithValue("@name", row.Cells["status"].Value);
                    int id_status = (int)command_status.ExecuteScalar();

                    using var command_check = new NpgsqlCommand() { Connection = connect };
                    command_check.CommandText = "select id_user from users where login = @login";
                    command_check.Parameters.AddWithValue("@login", row.Cells["login"].Value.ToString());
                    int? id_user = (int?)command_check.ExecuteScalar();

                    using var command_user = new NpgsqlCommand() { Connection = connect };
                    command_user.Parameters.AddWithValue("@age", row.Cells["age"].Value ?? DBNull.Value);
                    command_user.Parameters.AddWithValue("@status_id", id_status);
                    command_user.Parameters.AddWithValue("@login", row.Cells["login"].Value);

                    using var command_for_questions = new NpgsqlCommand() { Connection = connect };
                    command_for_questions.Parameters.AddWithValue("@link_id", row.Cells["link_id"].Value ?? DBNull.Value);
                    command_for_questions.Parameters.AddWithValue("@count_view", row.Cells["count_view"].Value ?? DBNull.Value);
                    command_for_questions.Parameters.AddWithValue("@count_like", row.Cells["count_like"].Value ?? DBNull.Value);

                    if (row.Tag != null) //if a tag of the mutable row is null, then this row is new
                    {
                        func_update_user(connect, command_user, id_user, ((List<object>)row.Tag)[0]);
                        func_update_question(command_for_questions, row.Cells["id_question"].Value);
                        UpdateUserInfoDataGrid(row, id_status);
                    }
                    else
                    {
                        if (id_user == null) id_user = func_insert_user(command_user);
                        int id_question = func_insert_question(command_for_questions, id_user);

                        row.Cells["id_question"].Value = id_question;
                        row.Cells["user_id"].Value = id_user;
                        row.Cells["status_id"].Value = id_status;
                    }
                    connect.Close();
                    var dataDict = new List<object>();
                    foreach (DataGridViewColumn column in data_grid_view.Columns)
                    {
                        dataDict.Add(row.Cells[column.Name].Value);
                    }

                    row.Tag = dataDict; //change a tag this row 

                    row.ErrorText = string.Empty; //clear error text
                    row.Cells["age"].ReadOnly = false;
                    row.Cells["status"].ReadOnly = false;
                }

            }
        }

        private int? func_insert_user(NpgsqlCommand command_user)
        {
            command_user.CommandText = @"insert into users(login, age, status_id) values (@login, @age, @status_id) returning id_user";
            return (int?)command_user.ExecuteScalar();
        }

        private int func_insert_question(NpgsqlCommand command_for_questions, int? id_user)
        {
            command_for_questions.CommandText = @"insert into questions(link_id, count_view, count_like, user_id) values (@link_id, @count_view, @count_like, @id_user) returning id_question";
            command_for_questions.Parameters.AddWithValue("@id_user", id_user);
            return (int)command_for_questions.ExecuteScalar();
        }

        private void func_update_user(NpgsqlConnection connection, NpgsqlCommand command_user, int? id_user, object id_question)
        {
            if (id_user == null) //если нового пользователя нету, то мы вставляем его 
            {
                command_user.CommandText = "insert into users(login, age, status_id) values (@login, @age, @status_id) returning id_user";

                id_user = (int)command_user.ExecuteScalar();
            }
            else
            {
                command_user.CommandText = "update users set age = @age, status_id = @status_id where login = @login";
                command_user.ExecuteNonQuery();
            }

            using var command_update_questions = new NpgsqlCommand() { Connection = connection };
            command_update_questions.CommandText = "update questions set user_id = @user_id where  id_question = @id_question";
            command_update_questions.Parameters.AddWithValue("@user_id", id_user);
            command_update_questions.Parameters.AddWithValue("@id_question", id_question);
            command_update_questions.ExecuteNonQuery();
        }

        private void func_update_question(NpgsqlCommand command_for_questions, object id_question)
        {
            command_for_questions.CommandText = @"update questions set link_id = @link_id, count_view = @count_view, count_like = @count_like
                                                        where id_question = @id_question";
            command_for_questions.Parameters.AddWithValue("@id_question", id_question);
            command_for_questions.ExecuteNonQuery();
        }

        private void UpdateUserInfoDataGrid(DataGridViewRow row, int id_status)
        {
            if (row.Cells["login"].Value.ToString() == ((List<object>)row.Tag)[5].ToString())
            {
                if ((int?)row.Cells["status_id"].Value != id_status) // if status is changed
                {
                    foreach (DataGridViewRow row_up in data_grid_view.Rows)
                    {
                        if (!row_up.IsNewRow)
                            if (row_up.Cells["login"].Value.ToString() == row.Cells["login"].Value.ToString()) // in all rows with this login change the status
                            {
                                row_up.Cells["status_id"].Value = id_status;
                                row_up.Cells["status"].Value = row.Cells["status"].Value;
                            }
                    }
                }

                if (row.Cells["age"].Value.ToString() != ((List<object>)row.Tag)[6].ToString())
                {
                    foreach (DataGridViewRow row_up in data_grid_view.Rows)
                    {
                        if (!row_up.IsNewRow)
                            if (row_up.Cells["login"].Value.ToString() == row.Cells["login"].Value.ToString()) // in all rows with this login change the status
                            {
                                row_up.Cells["age"].Value = row.Cells["age"].Value;
                            }
                    }
                }
            }
        }
        private void button_delete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in data_grid_view.SelectedRows) //delete selected row
                delete_one_row(row);
        }
        private void delete_one_row(DataGridViewRow row)
        {
            if (row.IsNewRow) return;
            using var connect = new NpgsqlConnection(connect_string);
            connect.Open();
            using var command_delete = new NpgsqlCommand("delete from questions where id_question = @id_question and user_id = @user_id", connect);
            command_delete.Parameters.AddWithValue("@id_question", row.Cells["id_question"].Value);
            command_delete.Parameters.AddWithValue("@film_name", row.Cells["user_id"].Value);
            command_delete.ExecuteNonQuery();
            connect.Close();
            data_grid_view.Rows.Remove(row);
        }

        private void status_load(DataTable data)
        {
            using var connect = new NpgsqlConnection(connect_string);
            connect.Open();
            using var command = new NpgsqlCommand("select name from statuses", connect);
            data.Load(command.ExecuteReader());
            connect.Close();
        }

        private void data_grid_view_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                using var connect = new NpgsqlConnection(connect_string);
                connect.Open();
                int? id_user = null;
                using (var command_check = new NpgsqlCommand() { Connection = connect })
                {
                    command_check.CommandText = "select id_user from users where login = @login";
                    command_check.Parameters.AddWithValue("@login", data_grid_view.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                    id_user = (int?)command_check.ExecuteScalar();
                }
                if (id_user == null)
                {
                    data_grid_view.Rows[e.RowIndex].Cells["age"].ReadOnly = false;
                    data_grid_view.Rows[e.RowIndex].Cells["status"].ReadOnly = false;
                    return;
                }
                using (var command_select_data = new NpgsqlCommand() { Connection = connect })
                {
                    command_select_data.CommandText = "select login, age, name from users inner join statuses s on users.status_id = s.id_status where login = @login";
                    command_select_data.Parameters.AddWithValue("@login", data_grid_view.Rows[e.RowIndex].Cells["login"].Value);
                    var reader = command_select_data.ExecuteReader();
                    bool check = reader.Read();
                    data_grid_view.Rows[e.RowIndex].Cells["age"].Value = reader["age"] ?? null;
                    data_grid_view.Rows[e.RowIndex].Cells["age"].ReadOnly = true;
                    data_grid_view.Rows[e.RowIndex].Cells["status"].Value = reader["name"] ?? null;
                    data_grid_view.Rows[e.RowIndex].Cells["status"].ReadOnly = true;
                }
                connect.Close();
            }
        }

        private void data_grid_view_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Поле имеет неверный тип", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Error);
           
        }
    }
}