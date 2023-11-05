namespace Bonsai.Harp.Visualizers
{
    partial class TimelineGraphView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.cursorStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.timeSpanStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.timeSpanValueLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.graph = new Bonsai.Harp.Visualizers.TimelineGraph();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cursorStatusLabel,
            this.timeSpanStatusLabel,
            this.timeSpanValueLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 218);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(400, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip";
            this.statusStrip.Visible = false;
            // 
            // cursorStatusLabel
            // 
            this.cursorStatusLabel.Name = "cursorStatusLabel";
            this.cursorStatusLabel.Size = new System.Drawing.Size(45, 17);
            this.cursorStatusLabel.Text = "Cursor:";
            // 
            // timeSpanStatusLabel
            // 
            this.timeSpanStatusLabel.Name = "timeSpanStatusLabel";
            this.timeSpanStatusLabel.Size = new System.Drawing.Size(47, 17);
            this.timeSpanStatusLabel.Text = "TimeSpan:";
            // 
            // timeSpanValueLabel
            // 
            this.timeSpanValueLabel.Name = "timeSpanValueLabel";
            this.timeSpanValueLabel.Size = new System.Drawing.Size(12, 17);
            this.timeSpanValueLabel.Text = "count";
            // 
            // graph
            // 
            this.graph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graph.Location = new System.Drawing.Point(0, 0);
            this.graph.Name = "graph";
            this.graph.ScrollGrace = 0D;
            this.graph.ScrollMaxX = 0D;
            this.graph.ScrollMaxY = 0D;
            this.graph.ScrollMaxY2 = 0D;
            this.graph.ScrollMinX = 0D;
            this.graph.ScrollMinY = 0D;
            this.graph.ScrollMinY2 = 0D;
            this.graph.Size = new System.Drawing.Size(400, 218);
            this.graph.TabIndex = 2;
            this.graph.MouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.graph_MouseMoveEvent);
            this.graph.MouseClick += new System.Windows.Forms.MouseEventHandler(this.graph_MouseClick);
            // 
            // TimelineGraphView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.graph);
            this.Controls.Add(this.statusStrip);
            this.Name = "TimelineGraphView";
            this.Size = new System.Drawing.Size(400, 240);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private Bonsai.Harp.Visualizers.TimelineGraph graph;
        private System.Windows.Forms.ToolStripStatusLabel cursorStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel timeSpanStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel timeSpanValueLabel;
    }
}
