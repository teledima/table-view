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
            data_grid_view.Rows.Clear();
            data_grid_view.Columns.Clear();
            data_grid_view.Columns.Add("actor_name", "actor_name");
            data_grid_view.Columns.Add("film_name", "film_name");
            data_grid_view.Columns.Add("honorarium", "honorarium");
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
                    data_grid_view.Rows.Add(reader["actor_name"], reader["film_name"], reader["honorarium"]);
            }
            foreach (DataGridViewRow row in data_grid_view.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[1].Value != null && row.Cells[2].Value != null)
                {
                    var tag = new List<object>();
                    foreach (DataGridViewCell cell in row.Cells)
                        tag.Add(cell.Value);
                    row.Tag = tag;
                }
            }
        }

        private void data_grid_view_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            var row = data_grid_view.Rows[e.RowIndex];
            if (data_grid_view.IsCurrentRowDirty)
            {
                foreach (DataGridViewCell cell in data_grid_view.Rows[e.RowIndex].Cells)
                {
                    if (cell.Value is null) return;
                }
                if (!e.Cancel)
                {
                    using var connect = new NpgsqlConnection(connect_string);
                    connect.Open();
                    using var command = new NpgsqlCommand() { Connection = connect };
                    command.Parameters.AddWithValue("@actor_name", row.Cells["actor_name"].Value);
                    command.Parameters.AddWithValue("@film_name", row.Cells["film_name"].Value);
                    command.Parameters.AddWithValue("@honorarium", int.Parse(row.Cells["honorarium"].Value.ToString()));
                    if (row.Tag != null)
                    {

                        command.CommandText = @"UPDATE view_cinema SET actor_name = @actor_name, film_name = @film_name, honorarium = @honorarium
                                                        where actor_name = @old_actor_name and film_name = @old_film_name";
                        command.Parameters.AddWithValue("@old_actor_name", ((List<object>)row.Tag)[0]);
                        command.Parameters.AddWithValue("@old_film_name", ((List<object>)row.Tag)[1]);
                    }
                    else
                    {
                        command.CommandText = @"INSERT INTO view_cinema(actor_name, film_name, honorarium) VALUES (@actor_name, @film_name, @honorarium)";
                    }
                    int check = CheckDuplicates(row.Cells["actor_name"].Value.ToString(), row.Cells["film_name"].Value.ToString(), row.Index);
                    if (check >= 0)
                    {
                        data_grid_view.Rows[check].Cells["honorarium"].Value = int.Parse(data_grid_view.Rows[check].Cells["honorarium"].Value.ToString()) + int.Parse(row.Cells["honorarium"].Value.ToString());
                        row.SetValues(string.Empty, string.Empty, string.Empty);
                    }
                    command.ExecuteNonQuery();
                    connect.Close();
                    var dataDict = new List<object>();
                    foreach (var columnsName in new[] { "actor_name", "film_name", "honorarium" })
                    {
                        dataDict.Add(row.Cells[columnsName].Value);
                    }

                    row.Tag = dataDict;

                    row.ErrorText = string.Empty;
                }
            }
        }
        private void button_delete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in data_grid_view.SelectedRows)
                delete_one_row(row);
            delete_empty_row();
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
                    if (row.Cells[0].Value.ToString() == actor_name && row.Cells[1].Value.ToString() == film_name)
                        return row.Index;
            return -1;
        }

        private void data_grid_view_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            //DataGridViewRow row_deleted = null;
            //foreach (DataGridViewRow row in data_grid_view.Rows)
            //    if (!row.IsNewRow)
            //        if (row.Cells[0].Value.ToString() == string.Empty && row.Cells[1].Value.ToString() == string.Empty && row.Cells[2].Value.ToString() == string.Empty)
            //            row_deleted = row;
            //if (row_deleted != null)
            //    data_grid_view.Rows.Remove(row_deleted);
        }
    }
}