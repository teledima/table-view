namespace lab6
{
    partial class FormPresentantion
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.data_grid_view = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.data_grid_view)).BeginInit();
            this.SuspendLayout();
            // 
            // data_grid_view
            // 
            this.data_grid_view.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.data_grid_view.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.data_grid_view.ColumnHeadersHeight = 29;
            this.data_grid_view.Location = new System.Drawing.Point(12, 7);
            this.data_grid_view.Name = "data_grid_view";
            this.data_grid_view.RowHeadersWidth = 51;
            this.data_grid_view.RowTemplate.Height = 24;
            this.data_grid_view.Size = new System.Drawing.Size(876, 305);
            this.data_grid_view.TabIndex = 0;
            this.data_grid_view.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.data_grid_view_CellEndEdit);
            this.data_grid_view.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.data_grid_view_RowValidating);
            this.data_grid_view.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.data_grid_view_UserDeletingRow);
            // 
            // FormPresentantion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 336);
            this.Controls.Add(this.data_grid_view);
            this.MinimumSize = new System.Drawing.Size(600, 200);
            this.Name = "FormPresentantion";
            this.ShowIcon = false;
            this.Text = "Табличное представление";
            ((System.ComponentModel.ISupportInitialize)(this.data_grid_view)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView data_grid_view;
    }
}

