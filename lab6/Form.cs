using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Npgsql;

namespace lab6
{
    public partial class FormPresentantion : Form
    {
        private readonly string connect_string = "Host = localhost; Username = postgres; password = postgres; Database = postgres";

        public FormPresentantion()
        {
            InitializeComponent();
            InitializePresentation();
        }
        private void InitializePresentation()
        {
            var sex = new DataGridViewComboBoxColumn()
            {
                Name = "sex",
                ReadOnly = false,
            };
            sex.Items.Add("man");
            sex.Items.Add("women");

            data_grid_view.Rows.Clear();
            data_grid_view.Columns.Clear();
            data_grid_view.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "actor_name",
                HeaderText = "actor_name",
                ValueType = typeof(string)
            });
            data_grid_view.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "age",
                HeaderText = "age",
                ValueType = typeof(int)
            });
            data_grid_view.Columns.Add(sex);
            data_grid_view.Columns.Add(new DataGridViewTextBoxColumn()
            { 
                Name = "film_name",
                HeaderText = "film_name",
                ValueType = typeof(string)
            });
            data_grid_view.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "running_time",
                HeaderText = "running_type",
                ValueType = typeof(int)
            });
            data_grid_view.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "budget",
                HeaderText = "budget",
                ValueType = typeof(int)
            });
            data_grid_view.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "honorarium",
                HeaderText = "honorarium",
                ValueType = typeof(int)
            });
            using (var connect = new NpgsqlConnection(connect_string))
            {
                connect.Open();
                var sCommand = new NpgsqlCommand
                {
                    Connection = connect,
                    CommandText = @"SELECT * from view_cinema"
                };
                var reader = sCommand.ExecuteReader();
                while (reader.Read())
                    data_grid_view.Rows.Add(reader["actor_name"], reader["age"], reader["sex"], reader["film_name"], reader["running_time"], reader["budget"], reader["honorarium"]); //fill data_grid_view
            }
            foreach (DataGridViewRow row in data_grid_view.Rows)
            {
                var tag = new List<object>();
                foreach (DataGridViewCell cell in row.Cells)
                    tag.Add(cell.Value);
                 row.Tag = tag;
                if (row.IsNewRow) row.Tag = null;
                //assign a tag for each added row equal his values
            }
        }

        private void data_grid_view_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            var row = data_grid_view.Rows[e.RowIndex];
            if (data_grid_view.IsCurrentRowDirty)
            {
                if (!e.Cancel)
                {
                    using var connect = new NpgsqlConnection(connect_string);
                    connect.Open();
                    using var command = new NpgsqlCommand() { Connection = connect };
                    command.Parameters.AddWithValue("@actor_name", row.Cells["actor_name"].Value);
                    command.Parameters.AddWithValue("@age", row.Cells["age"].Value);
                    command.Parameters.AddWithValue("@sex", row.Cells["sex"].Value);
                    command.Parameters.AddWithValue("@film_name", row.Cells["film_name"].Value);
                    command.Parameters.AddWithValue("@running_time", row.Cells["running_time"].Value);
                    command.Parameters.AddWithValue("@budget", row.Cells["budget"].Value);
                    command.Parameters.AddWithValue("@honorarium", int.Parse(row.Cells["honorarium"].Value.ToString()));
                    if (row.Tag != null) //if a tag of the mutable row is null, then this row is new
                    {

                        command.CommandText = @"update view_cinema set actor_name = @actor_name, age=@age, sex=@sex, film_name=@film_name, running_time=@running_time, budget=@budget, honorarium=@honorarium
                                                where actor_name = @old_actor_name and film_name = @old_film_name";
                        command.Parameters.AddWithValue("@old_actor_name", ((List<object>)row.Tag)[0]);
                        command.Parameters.AddWithValue("@old_film_name", ((List<object>)row.Tag)[3]);
                    }
                    else
                    {
                        command.CommandText = @"insert into view_cinema(actor_name, age, sex, film_name, running_time, budget) VALUES (@actor_name, @age, @sex, @film_name, @running_time, @budget)";
                    }
                    int check = CheckDuplicates(row.Cells["actor_name"].Value.ToString(), row.Cells["film_name"].Value.ToString(), row.Index); //if there is the row in data_grid, then the function return index second row, else return -1 
                    if (check >= 0) //if there is duplicates
                    {
                        data_grid_view.Rows[check].Cells["honorarium"].Value = int.Parse(data_grid_view.Rows[check].Cells["honorarium"].Value.ToString()) + int.Parse(row.Cells["honorarium"].Value.ToString()); //update honorarium
                        row.SetValues(string.Empty, string.Empty, string.Empty); //erase the values
                    }
                    command.ExecuteNonQuery();
                    connect.Close();
                    foreach (DataGridViewRow row_t in data_grid_view.Rows)
                    {
                        var tag = new List<object>();
                        foreach (DataGridViewCell cell in row_t.Cells)
                            tag.Add(cell.Value);
                        row_t.Tag = tag;
                        if (row_t.IsNewRow) row_t.Tag = null;
                        //assign a tag for each added row equal his values
                    }

                    row.ErrorText = string.Empty; //clear error text
                }
            }
        }
        private void button_delete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in data_grid_view.SelectedRows) //delete selected row
                delete_one_row(row);
        }
        private void delete_empty_row()
        {
            DataGridViewRow row_deleted = null;
            foreach (DataGridViewRow row in data_grid_view.Rows)
                if (!row.IsNewRow)
                    if (row.Cells[0].Value.ToString() == string.Empty && row.Cells[1].Value.ToString() == string.Empty && row.Cells[2].Value.ToString() == string.Empty)
                        row_deleted = row;
            if (row_deleted != null)
                data_grid_view.Rows.Remove(row_deleted);
        }
        private void delete_one_row(DataGridViewRow row)
        {
            using (var connect = new NpgsqlConnection(connect_string))
            {
                connect.Open();
                using var command_delete = new NpgsqlCommand("delete from view_cinema where actor_name = @actor_name and film_name = @film_name", connect);
                command_delete.Parameters.AddWithValue("@actor_name", data_grid_view.SelectedRows[0].Cells["actor_name"].Value.ToString());
                command_delete.Parameters.AddWithValue("@film_name", data_grid_view.SelectedRows[0].Cells["film_name"].Value.ToString());
                command_delete.ExecuteNonQuery();
                connect.Close();
            }
            data_grid_view.Rows.Remove(row);
        }

        private void data_grid_view_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var cell = data_grid_view[e.ColumnIndex, e.RowIndex];
            var value = cell.Value?.ToString();
            if (string.IsNullOrWhiteSpace(value))
            {
                if (e.ColumnIndex == 0)
                    cell.ErrorText = "Имя не может быть пустым";
                else if (e.ColumnIndex == 1)
                    cell.ErrorText = "Название фильма не может быть пустым";
            }
            else if (cell.ColumnIndex == 2)
            {
                if (int.TryParse(cell.Value.ToString(), out _))
                {
                    if (int.Parse(value) <= 0)
                    {
                        cell.ErrorText = "Гонорар не может быть отрицательным или нулевым";
                    }
                }
                else
                {
                    cell.ErrorText = "Гонорар не должен содержать символы, отличные от цифр";
                }
            }
            else
            {
                cell.ErrorText = string.Empty;
            }
            RewriteRowErrorText(e.RowIndex);
        }
        private void RewriteRowErrorText(int row_index)
        {
            var row = data_grid_view.Rows[row_index];
            List<string> error_columns = new List<string>();
            foreach (DataGridViewCell cell in row.Cells)
                if (cell.Value is null)
                    error_columns.Add(cell.OwningColumn.HeaderText);
            if (error_columns.Count > 0)
            {
                var builder = new StringBuilder("Данные в столбце/столбцах ");
                foreach (string error_column in error_columns)
                    builder.Append(error_column + " ");
                builder.Append(" не могут быть пустыми");
                row.ErrorText = builder.ToString();
            }
            else
            {
                if (data_grid_view.IsCurrentRowDirty)
                    row.ErrorText = "Эта строка не зафиксирована";
                else
                    row.ErrorText = string.Empty;
            }
        }

        private int CheckDuplicates(string actor_name, string film_name, int index_no_check)
        {
            foreach (DataGridViewRow row in data_grid_view.Rows)
                if (row.Index != index_no_check && !row.IsNewRow)
                    if (row.Cells[0].Value.ToString() == actor_name && row.Cells[1].Value.ToString() == film_name) //if rows have identical actor_name and film_name, then these rows are duplicates
                        return row.Index;
            return -1;
        }

        private void button_refresh_Click(object sender, EventArgs e) //"Refresh" delete a empty rows 
        {
            delete_empty_row();
        }
    }
}