namespace ECOLOGCarSimulator
{
    partial class CarSimulator
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Button InsertButten;
            InsertButten = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // InsertButten
            // 
            InsertButten.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            InsertButten.Location = new System.Drawing.Point(66, 90);
            InsertButten.Name = "InsertButton";
            InsertButten.Size = new System.Drawing.Size(139, 90);
            InsertButten.TabIndex = 0;
            InsertButten.Text = "Insert";
            InsertButten.UseVisualStyleBackColor = true;
            InsertButten.Click += new System.EventHandler(this.InsertButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(InsertButten);
            this.Name = "CarSimulation";
            this.Text = "CarSimulation";
            this.ResumeLayout(false);

        }

        #endregion
    }
}

