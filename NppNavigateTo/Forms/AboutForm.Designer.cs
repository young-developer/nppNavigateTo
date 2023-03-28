namespace NavigateTo.Plugin.Namespace
{
    partial class AboutForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Title = new System.Windows.Forms.Label();
            this.Description = new System.Windows.Forms.Label();
            this.GitHubLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.Location = new System.Drawing.Point(47, 9);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(152, 29);
            this.Title.TabIndex = 0;
            this.Title.Text = "placeholder";
            // 
            // Description
            // 
            this.Description.AutoSize = true;
            this.Description.Location = new System.Drawing.Point(32, 56);
            this.Description.Name = "Description";
            this.Description.Size = new System.Drawing.Size(263, 64);
            this.Description.TabIndex = 1;
            this.Description.Text = "NavigateTo is a plugin for making it easy to\r\nmanage large numbers of open files," +
    "\r\nsearch for other files to open,\r\nand find menu commands.";
            // 
            // GitHubLink
            // 
            this.GitHubLink.AutoSize = true;
            this.GitHubLink.LinkArea = new System.Windows.Forms.LinkArea(40, 17);
            this.GitHubLink.Location = new System.Drawing.Point(35, 147);
            this.GitHubLink.Name = "GitHubLink";
            this.GitHubLink.Size = new System.Drawing.Size(238, 35);
            this.GitHubLink.TabIndex = 2;
            this.GitHubLink.TabStop = true;
            this.GitHubLink.Text = "Something wrong?\r\nRaise an issue in the Github repository.";
            this.GitHubLink.UseCompatibleTextRendering = true;
            this.GitHubLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.GitHubLink_LinkClicked);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 225);
            this.Controls.Add(this.GitHubLink);
            this.Controls.Add(this.Description);
            this.Controls.Add(this.Title);
            this.Name = "AboutForm";
            this.Text = "About NavigateTo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.Label Description;
        private System.Windows.Forms.LinkLabel GitHubLink;
    }
}