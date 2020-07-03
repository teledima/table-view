using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace lab6
{
    public partial class FormPresentantion : Form
    {
        private Brainly_info db = new Brainly_info();
        public FormPresentantion()
        {
            InitializeComponent();
            InitializePresentation();
        }
        private void InitializePresentation()
        {
            List<subjects> subjects = db.subjects.ToList();
            data_grid_view.Rows.Clear();
            data_grid_view.Columns.Clear();
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
                    Name = "subject_id",
                    HeaderText = "subject_id",
                    Visible = false,
                    ValueType = typeof(int)
                });
                data_grid_view.Columns.Add(new DataGridViewComboBoxColumn
                {
                    Name = "subject",
                    HeaderText = "subject",
                    ValueType = typeof(string),
                    DataSource = db.subjects.ToList(),
                    Width = 150,
                    DisplayMember = "name"
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
                    DataSource = db.statuses.ToList(),
                    Width = 150,
                    DisplayMember = "name"
                });
            }

            foreach (questions question in db.questions.Include("users"))
            {   
                    data_grid_view.Rows.Add(question.id_question, question.link_id, question.count_view,
                        question.subjects.id, question.subjects.name,
                        question.users.id_user, question.users.login, question.users.age,
                        question.users.statuses.id_status, question.users.statuses.name);
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
                if (row.Cells["status"].Value == null) row.Cells["status"].Value = "Hide";
                if (row.Cells["count_view"].Value == null || row.Cells["count_view"].Value is DBNull) row.Cells["count_view"].Value = 0;
                if (row.Cells["login"].Value == null || row.Cells["subject"].Value == null || row.Cells["age"].Value == null || row.Cells["age"].Value is DBNull || row.Cells["link_id"].Value == null || row.Cells["link_id"].Value is DBNull ||
                    (int)row.Cells["age"].Value < 0 || (int)row.Cells["link_id"].Value < 0)
                {
                    e.Cancel = true;
                    if (MessageBox.Show("Введены некорректные данные \n\nВернуть предыдущие значения?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    {
                        row.Cells["link_id"].Value = ((List<object>)row.Tag)[1];
                        row.Cells["subject"].Value = ((List<object>)row.Tag)[4];
                        row.Cells["login"].Value = ((List<object>)row.Tag)[6];
                        row.Cells["age"].Value = ((List<object>)row.Tag)[7];
                    }
                }
                if (!e.Cancel)
                {
                    statuses status = db.statuses.Where(status => status.name == (string)row.Cells["status"].Value).First();
                    users user = db.users.Where(user => user.login == (string)row.Cells["login"].Value).FirstOrDefault();
                    questions question = db.questions.Where(question => question.id_question == (int?)row.Cells["id_question"].Value).FirstOrDefault();
                    subjects subject = db.subjects.Where(subject => subject.name == (string)row.Cells["subject"].Value).First();

                    user = UserSave(user, (string)row.Cells["login"].Value, (int)row.Cells["age"].Value, status.id_status);
                    question = QuestionSave(question, (int)row.Cells["link_id"].Value, (int)row.Cells["count_view"].Value, subject.id, user.id_user);

                    if (row.Cells["id_question"].Value != null) //if a tag of the mutable row is null, then this row is new
                    {
                        UpdateUserInfoDataGrid(row, status.id_status);
                    }
                    else
                    {
                        row.Cells["id_question"].Value = question.id_question;
                        row.Cells["id_question"].Value = subject.id;
                        row.Cells["user_id"].Value = user.id_user;
                        row.Cells["status_id"].Value = user.statuses.id_status;
                    }

                    var list = new List<object>();
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        list.Add(cell.Value);
                    }
                    row.Tag = list; //change a tag this row 

                    row.ErrorText = string.Empty; //clear error text
                    row.Cells["age"].ReadOnly = false;
                    row.Cells["status"].ReadOnly = false;
                }

            }
        }

        private users UserSave(users user, string login, int age, int id_status)
        {
            if (user == null) user = db.Add(
                new users(login, age, id_status))
                .Entity;
            else
            {
                user.login = login;
                user.age = age;
                user.status_id = id_status;
            }
            return user;
        }

        private questions QuestionSave(questions question, int link_id, int count_view, int subject_id, int user_id)
        {
            if (question == null)
                question = db.Add(new questions(link_id, count_view, subject_id, user_id)).Entity;
            else
            {
                question.link_id = link_id;
                question.count_view = count_view;
                question.subject_id = subject_id;
                question.user_id = user_id;
            }
            db.SaveChanges();
            return question;
        }

        private void UpdateUserInfoDataGrid(DataGridViewRow row, int id_status)
        {
            if (row.Cells["login"].Value.ToString() == ((List<object>)row.Tag)[6].ToString())
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

                if (row.Cells["age"].Value.ToString() != ((List<object>)row.Tag)[7].ToString())
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

        private void data_grid_view_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 6)
            {
                users user = db.users.Where(user => user.login == (string)data_grid_view.Rows[e.RowIndex].Cells[e.ColumnIndex].Value).FirstOrDefault();
                if (user == null)
                {
                    data_grid_view.Rows[e.RowIndex].Cells["age"].ReadOnly = false;
                    data_grid_view.Rows[e.RowIndex].Cells["status"].ReadOnly = false;
                    return;
                }
                data_grid_view.Rows[e.RowIndex].Cells["user_id"].Value = user.id_user;
                data_grid_view.Rows[e.RowIndex].Cells["age"].Value = user.age;
                data_grid_view.Rows[e.RowIndex].Cells["age"].ReadOnly = true;
                data_grid_view.Rows[e.RowIndex].Cells["status_id"].Value = user.status_id;
                data_grid_view.Rows[e.RowIndex].Cells["status"].Value = user.statuses.name;
                data_grid_view.Rows[e.RowIndex].Cells["status"].ReadOnly = true;
            }
        }

        private void data_grid_view_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || e.Row.Cells["id_question"].Value == null) return;
            db.questions.Remove(db.questions.Find((int)e.Row.Cells["id_question"].Value));
            db.SaveChanges();
        }
    }
}