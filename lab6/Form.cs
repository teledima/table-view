using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Npgsql;

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
            data_grid_view.Columns.Add(new DataGridViewColumn
            {
                Name = "id_cinema",
                Visible = false
            });
            data_grid_view.Columns.Add("cinema_name", "cinema_name");
            data_grid_view.Columns.Add("locate", "locate");
            data_grid_view.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "is_parking",
                ValueType = Type.GetType("bool"),
                TrueValue = true,
                FalseValue = true,
            });
            data_grid_view.Columns.Add(new DataGridViewColumn
            {
                Name = "id_hall",
                Visible = false
            });
            data_grid_view.Columns.Add("hall_name", "hall_name");
            data_grid_view.Columns.Add("capacity", "capacity");
            data_grid_view.Columns.Add("area", "area");

            using (var connect = new NpgsqlConnection(connect_string))
            {
                connect.Open();
                var sCommand = new NpgsqlCommand
                {
                    Connection = connect,
                    CommandText = @"select * from cinemas inner join halls h on cinemas.id_hall = h.id_hall"
                };
                var reader = sCommand.ExecuteReader();
                while (reader.Read())
                    data_grid_view.Rows.Add(reader["id"], reader["cinema_name"], reader["locate"], reader["is_parking"], reader["hall_name"], reader["capacity"], reader["area"]); //fill data_grid_view
            }
            foreach (DataGridViewRow row in data_grid_view.Rows)
            {
                if (row.Tag != null)
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
                foreach (DataGridViewCell cell in data_grid_view.Rows[e.RowIndex].Cells)
                {
                    if (cell.Value is null) return;
                }
                if (!e.Cancel)
                {
                    using var connect = new NpgsqlConnection(connect_string);
                    connect.Open();
                    using var command_for_cinema = new NpgsqlCommand() { Connection = connect };
                    command_for_cinema.Parameters.AddWithValue("@id_cinema", NpgsqlTypes.NpgsqlDbType.Integer, int.Parse(row.Cells["id_cinema"].Value.ToString()));
                    command_for_cinema.Parameters.AddWithValue("@cinema_name", row.Cells["cinema_name"].Value);
                    command_for_cinema.Parameters.AddWithValue("@locate", row.Cells["locate"].Value);
                    command_for_cinema.Parameters.AddWithValue("@is_parking", bool.Parse(row.Cells["locate"].Value.ToString()));
                    using var command_for_hall = new NpgsqlCommand() { Connection = connect };
                    command_for_hall.Parameters.AddWithValue("@id_hall", int.Parse(row.Cells["id_hall"].Value.ToString()));
                    command_for_hall.Parameters.AddWithValue("@hall_name", row.Cells["hall_name"].Value);
                    command_for_hall.Parameters.AddWithValue("@capacity", int.Parse(row.Cells["capacity"].Value.ToString()));
                    command_for_hall.Parameters.AddWithValue("@area", int.Parse(row.Cells["area"].Value.ToString()));
                    if (row.Tag != null) //if a tag of the mutable row is null, then this row is new
                    {

                        command_for_cinema.CommandText = @"UPDATE cinemas SET cinema_name = @cinema_name, locate = @locate, is_parking = @is_parking
                                                        where id_cinema = @id_cinema";

                        command_for_hall.CommandText = @"update halls set hall_name = @hall_name, capacity = @capacity, area = @area
                                                 where id_hall = @id_hall";
                    }
                    else
                    {
                        command_for_cinema.CommandText = @"insert into cinemas(cinema_name, locate, id_hall) VALUES (@cinema_name, @locate, @is_parking)";
                        command_for_hall.CommandText = @"INSERT INTO view_cinema(actor_name, film_name, honorarium) VALUES (@actor_name, @film_name, @honorarium)";
                    }
                    int check = CheckDuplicates(row.Cells["actor_name"].Value.ToString(), row.Cells["film_name"].Value.ToString(), row.Index); //if there is the row in data_grid, then the function return index second row, else return -1 
                    if (check >= 0) //if there is duplicates
                    {
                        data_grid_view.Rows[check].Cells["honorarium"].Value = int.Parse(data_grid_view.Rows[check].Cells["honorarium"].Value.ToString()) + int.Parse(row.Cells["honorarium"].Value.ToString()); //update honorarium
                        row.SetValues(string.Empty, string.Empty, string.Empty); //erase the values
                    }
                    command_for_cinema.ExecuteNonQuery();
                    connect.Close();
                    var dataDict = new List<object>();
                    foreach (var columnsName in new[] { "actor_name", "film_name", "honorarium" })
                    {
                        dataDict.Add(row.Cells[columnsName].Value);
                    }

                    row.Tag = dataDict; //change a tag this row 

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