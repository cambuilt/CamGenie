using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using Leadtools;
using Leadtools.Controls;
using Leadtools.Codecs;
using Leadtools.Forms;
using Leadtools.Forms.Ocr;
using Leadtools.Forms.DocumentWriters;
using Leadtools.Drawing;
using Leadtools.Demos;
using Leadtools.Demos.Dialogs;
using Leadtools.ImageProcessing;
using Leadtools.ImageProcessing.Core;
using Leadtools.Services;
using Leadtools.Services.Raster.DataContracts;
using Leadtools.Services.ImageProcessing.ServiceContracts;

namespace CamGenie
{
    public partial class MainForm : Form
    {
        private IOcrEngine ocrEngineProfessional, ocrEngineAdvantage;
        public IOcrPage ocrPageProfessional, lineOcrPage = null;
        private Leadtools.LeadRect currentHighlightRect = Leadtools.LeadRect.Empty;
        private ImageViewerZoomToInteractiveMode zoom;
        private RasterCodecs codecs = new RasterCodecs();
        private string batchPath = "", batchType = "PhoneBill", batchName = "", batchTableName = "Phone", batchDateField = "Calldate", zoneLineFile = "", callDateYear = "", versionNum = "", connectionString = "";
        public string[] images;
        private string[] imageFilePaths;
        private int clickedY = -1, clickedX = -1, contextClickedY = -1, contextClickedX = -1, zoneIndex = 0, mouseUpY = 0, mouseUpX = 0;
        private int zonesPerLine = 5, fadeCounter = 0, currentPageNumber = 1;
        private bool openingBatch = false, newZoneLine = false, hadLoadZoneLineError = false, modifiedImage = false;
        private OcrZone selectedZone = new OcrZone();
        private LineRemove lineRemove = null;
        
        public MainForm()
        {
            InitializeComponent();
            InitInteractiveModes();
            InitializeViewer();
            InitializeZoomComboBox();
            Support.SetLicense();   // Set up LEADTOOLS license.
            toolStripComboBoxSpacing.SelectedIndex = 17;  // Default line spacing is 12.
            versionNum = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + "." + Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString();
            this.Text = "CamGenie " + versionNum;
            connectionString = "Provider=SQLOLEDB;Data Source=ATR-LSB-LSS01;Initial Catalog=CamGenie;User ID=CMTS;Password=gErn8@f0Rm72";

			// Add batch recently opened to File menu.
            if (Properties.Settings.Default.RecentBatch1 != "")
            {
                fileToolStripMenuItem.DropDownItems.Insert(5, new ToolStripSeparator());
                ToolStripButton recentBath1 = new ToolStripButton(Properties.Settings.Default.RecentBatch1);
                recentBath1.Click += new EventHandler(toolStripButtonRecentBatch);
                fileToolStripMenuItem.DropDownItems.Insert(6, recentBath1);
            }

			// Add second recent batch only if not the same as the first.
            if (Properties.Settings.Default.RecentBatch2 != "" && Properties.Settings.Default.RecentBatch2 != Properties.Settings.Default.RecentBatch1)
            {
                ToolStripButton recentBath2 = new ToolStripButton(Properties.Settings.Default.RecentBatch2);
                recentBath2.Click += new EventHandler(toolStripButtonRecentBatch);
                fileToolStripMenuItem.DropDownItems.Insert(7, recentBath2);
            }
        }

        private void InitializeViewer()
        {
            // Set Scale-to-Gray
            RasterPaintProperties properties = RasterPaintProperties.Default;
            properties.PaintDisplayMode = RasterPaintDisplayModeFlags.Bicubic | RasterPaintDisplayModeFlags.ScaleToGray;
            properties.PaintEngine = RasterPaintEngine.GdiPlus;
            properties.UsePaintPalette = true;
            viewer.PaintProperties = properties;
        }

        private void InitializeZoomComboBox()
        {
			// Populate drop down list with zoom levels.
            zoomComboBox.Items.Add("Fit");
            zoomComboBox.Items.Add("Page Width");
            int[] initialValues = { 100, 25, 30, 35, 40, 50, 60, 75 };

            foreach (int i in initialValues)
                zoomComboBox.Items.Add(i + "%");

            for (int i = 125; i <= 200; i += 25)
                zoomComboBox.Items.Add(i + "%");

            zoomComboBox.SelectedIndex = 0;
        }

        private void InitInteractiveModes()
        {
			// Create ability to draw zone rectangles.
            viewer.InteractiveModes.BeginUpdate();
            viewer.InteractiveModes.Clear();
            zoom = new ImageViewerZoomToInteractiveMode();
            zoom.MouseButtons = System.Windows.Forms.MouseButtons.Left;
            zoom.RubberBandCompleted += new EventHandler<ImageViewerRubberBandEventArgs>(zoomRubberBandCompleted);
            viewer.InteractiveModes.Add(zoom);
            viewer.InteractiveModes.EndUpdate();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!DesignMode)
            {
                BeginInvoke(new MethodInvoker(Startup));
            }

            base.OnLoad(e);
        }

        private void Startup()
        {
			// Create OCR engine objects.
            ocrEngineProfessional = OcrEngineManager.CreateEngine(OcrEngineType.Professional, false);
            ocrEngineAdvantage = OcrEngineManager.CreateEngine(OcrEngineType.Advantage, false);
            StartEngine();

			// If there is at least one recent batch, open it automatically at startup.
            if (fileToolStripMenuItem.DropDownItems.Count > 5)
            {
                ToolStripButton recentBatch1 = (ToolStripButton)fileToolStripMenuItem.DropDownItems[6];
                toolStripButtonRecentBatch(recentBatch1, null);
            }
        }

        public void LoadImage()
        {
            try
            {
				// Save previously edited zones before loading a new image.
                SavePageZones();

				// Save TIF image file if it was cleaned up with deskew, noise removal, etc.
                if (modifiedImage)
                {
                    SaveImage();
                    modifiedImage = false;
                }

				// Get rid of backup file used for undo of image file cleanup changes.
                if (File.Exists(imageFilePaths[currentPageNumber - 1] + ".bak"))
                    File.Delete(imageFilePaths[currentPageNumber - 1] + ".bak");

                selectedZone = new OcrZone();
                currentPageNumber = int.Parse(toolStripTextBoxPageNumber.Text);
                toolStripStatusLabel.Text = imageFilePaths[currentPageNumber - 1];
                // Use the new RasterizeDocumentOptions to default loading document files at 300 DPI.
                codecs.Options.RasterizeDocument.Load.XResolution = 300;
                codecs.Options.RasterizeDocument.Load.YResolution = 300;

                RasterImage image = codecs.Load(images[currentPageNumber - 1]);
                if (image.XResolution < 150)
                    image.XResolution = 150;

                if (image.YResolution < 150)
                    image.YResolution = 150;

				// Get rid of old fresh IOcrPage object and create a new one.
				if (ocrPageProfessional != null)
                {
                    ocrPageProfessional.Dispose();
                    ocrPageProfessional = null;
                }

                viewer.Image = image;

                if (ocrEngineProfessional.IsStarted)
                    ocrPageProfessional = ocrEngineProfessional.CreatePage(image, OcrImageSharingMode.None);

                currentHighlightRect = Leadtools.LeadRect.Empty;

				// Zoom this image to the currently selected zoom level.
                zoomComboBox_SelectedIndexChanged(zoomComboBox, new EventArgs());
				// Load zones for this page if they exist.
                LoadPageZones();
            }
            catch (Exception ex)
            {
                Messager.ShowFileOpenError(this, "Load error", ex);
            }
            finally
            {
                viewer.Invalidate();
            }
        }

        private void SavePageZones()
        {
			// Save zones if there are any.
            if (ocrPageProfessional != null && ocrPageProfessional.Zones.Count > 0)
            {
				// Save zones to an ozf file corresponding to the current page.
                string pageImageFileRoot = images[currentPageNumber - 1];
                pageImageFileRoot = pageImageFileRoot.Substring(pageImageFileRoot.LastIndexOf("\\") + 1);
                pageImageFileRoot = pageImageFileRoot.Substring(0, pageImageFileRoot.LastIndexOf("."));
                string pageZonesFile = batchPath.Replace(":", ":\\") + "\\" + pageImageFileRoot + ".ozf";
                ocrPageProfessional.SaveZones(pageZonesFile);
            }
        }

        private void LoadPageZones()
        {
			// If a zones file (.ozf) exists, load the zones for the current image.
            int pageNumber = int.Parse(toolStripTextBoxPageNumber.Text);
            string pageImageFileRoot = images[pageNumber - 1];
            pageImageFileRoot = pageImageFileRoot.Substring(pageImageFileRoot.LastIndexOf("\\") + 1);
            pageImageFileRoot = pageImageFileRoot.Substring(0, pageImageFileRoot.LastIndexOf("."));
            string pageZonesFile = batchPath.Replace(":", ":\\") + "\\" + pageImageFileRoot + ".ozf";
            ocrPageProfessional.Zones.Clear();

            if (File.Exists(pageZonesFile))
            {
                ocrPageProfessional.LoadZones(pageZonesFile);
				// When zones are loaded, the user can clear them or delete the last line.
                toolStripButtonClearZones.Enabled = true;
                toolStripButtonDeleteLastLine.Enabled = true;
            }
        }

		// Set the current zone when selecting the corresponding image snippet from OCRQC based on its ID.
        public void FocusZone(int zoneID)
        {
            foreach (OcrZone zone in ocrPageProfessional.Zones)
            {
                if (zone.Id == zoneID)
                {
                    selectedZone = zone;
                    break;
                }
            }
            
            viewer.Invalidate();
        }

		// Set the current zone when selecting a grid cell from FinalQC. FinalQC knows the field name and
        // the top line coordinate, associating grid rows with zone lines from the pages ozf file.
        public void FocusZone(string fieldname, double lineTop)
        {
            foreach (OcrZone zone in ocrPageProfessional.Zones)
            {
                if (zone.Name.Contains(fieldname) && zone.Bounds.Top >= lineTop - 10 && zone.Bounds.Top <= lineTop + 10)
                {
                    selectedZone = zone;
                    break;
                }
            }

            viewer.Invalidate();
        }

		// Get the ID of a field given its name.
        private int GetFieldID(string fieldName)
        {
            int fieldID = 1;

            if (fieldName.StartsWith("Calldate"))
                fieldID = 1;
            else if (fieldName.StartsWith("Calltime"))
                fieldID = 2;
            else if (fieldName.StartsWith("Callto"))
                fieldID = 3;
            else if (fieldName.StartsWith("Callfrom"))
                fieldID = 4;
            else if (fieldName.StartsWith("City"))
                fieldID = 5;
            else if (fieldName.StartsWith("State"))
                fieldID = 6;
            else if (fieldName.StartsWith("CityState"))
                fieldID = 7;
            else if (fieldName.StartsWith("Duration"))
                fieldID = 8;
            else if (fieldName.StartsWith("Origination"))
                fieldID = 9;
            else if (fieldName.StartsWith("Calltype"))
                fieldID = 10;
            else if (fieldName.StartsWith("AccountNumber"))
                fieldID = 11;
            else if (fieldName.StartsWith("CheckNumber"))
                fieldID = 12;
            else if (fieldName.StartsWith("CardNumber"))
                fieldID = 13;
            else if (fieldName.StartsWith("Amount"))
                fieldID = 14;
            else if (fieldName.StartsWith("TransactionDate"))
                fieldID = 15;
            else if (fieldName.StartsWith("TransactionType"))
                fieldID = 16;
            else if (fieldName.StartsWith("RoutingNumber"))
                fieldID = 17;
            else if (fieldName.StartsWith("AccountHolder1"))
                fieldID = 18;
            else if (fieldName.StartsWith("AccountHolder2"))
                fieldID = 19;
            else if (fieldName.StartsWith("Description"))
                fieldID = 20;
            else if (fieldName.StartsWith("DepositAmount"))
                fieldID = 21;

            return fieldID;
        }

        private void StartEngine()
        {
			// Create OCR Engine objects
            try
            {
                ocrEngineProfessional = OcrEngineManager.CreateEngine(OcrEngineType.Advantage, false);
                ocrEngineProfessional.Startup(codecs, null, null, null);
                ocrEngineAdvantage = OcrEngineManager.CreateEngine(OcrEngineType.Advantage, false);
                ocrEngineAdvantage.Startup(codecs, null, null, null);
            }
            catch (Exception ex)
            {
                Messager.ShowError(this, ex);

                if (ocrEngineProfessional != null)
                    ocrEngineProfessional.Dispose();

                if (ocrEngineAdvantage != null)
                    ocrEngineAdvantage.Dispose();
            }
        }

        private void zoomComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Zoom the current image.
            // Get the size mode and scale factor.
            ControlSizeMode sizeMode = ControlSizeMode.None;
            double scaleFactor = viewer.ScaleFactor;

            string selected = zoomComboBox.SelectedItem.ToString();

            if (selected == "Fit")
            {
                sizeMode = ControlSizeMode.Fit;
                scaleFactor = 1.0;
            }
            else if (selected == "Page Width")
            {
                sizeMode = ControlSizeMode.FitWidth;
                scaleFactor = 1.0;
            }
            else if (selected == "100%")
            {
                scaleFactor = 1;
                sizeMode = ControlSizeMode.ActualSize;
            }
            else
            {
                int percentage = int.Parse(selected.Replace("%", ""));
                scaleFactor = (double)percentage / 100.0;
            }

            // Check if the size mode or scale factor has changed
            if (sizeMode != viewer.SizeMode || scaleFactor != viewer.ScaleFactor)
            {
                // yes, change it
                viewer.BeginUpdate();
                viewer.Zoom(sizeMode, scaleFactor, viewer.DefaultZoomOrigin);
                viewer.EndUpdate();
            }
        }

        void zoomRubberBandCompleted(object sender, ImageViewerRubberBandEventArgs e)
        {
			// Event fires when rectangle drawing is complete. Remember the size that was drawn.
            currentHighlightRect = viewer.ConvertRect(null, ImageViewerCoordinateType.Control, ImageViewerCoordinateType.Image, Leadtools.LeadRect.FromLTRB(e.Points[0].X, e.Points[0].Y, e.Points[1].X, e.Points[1].Y));

			// If the drawn rectangle is less than 200 high, it is a field and a zone is created by rubberBandCompleted.
            if (currentHighlightRect.Height < 200)
            {
				// Cancel the zoom that would normally happen.
                e.IsCanceled = true;
                rubberBandCompleted(sender, e);
            }
            else
            {
				// Big rectangle is drawn, so allow the viewer to zoom in and clear the current rectangle size which is only used for zones.
                currentHighlightRect = Leadtools.LeadRect.Empty;
            }
        }

        void rubberBandCompleted(object sender, ImageViewerRubberBandEventArgs e)
        {
            if (ocrPageProfessional == null)
                return;

            try
            {
				// Don't allow zooming when zone is being created.
                zoomComboBox.Enabled = false;

                if (viewer.Image != null)
                {
					// If the rectangle is big enough to be a field.
                    if (currentHighlightRect.Width > 2 && currentHighlightRect.Height > 2)
                    {
                        OcrZone zone = OcrTypeManager.CreateDefaultOcrZone();
						// ZoneProperties prompts for field name to connect to this zone.
                        ZoneProperties zoneProperties = new ZoneProperties();
						// Phone Bill or Bank Statement.
                        zoneProperties.batchType = batchType;
						// SelectedZones lets ZoneProperties know what fields have already been created
                        // so those are not displayed as choices in the displayed field list.
                        zoneProperties.SelectedZones = ocrPageProfessional.Zones;
						// Display ZoneProperties dialog close to the zone and relative to the current mouse position.
                        zoneProperties.Top = this.Top + viewer.Top + 64 + mouseUpY;
                        zoneProperties.Left = this.Left + mouseUpX;

                        if (zoneProperties.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                        {
                            zone.Name = zoneProperties.field;

							// Set character filters appropriate for the data type of the field.
                            if (zone.Name == "Calldate" || zone.Name == "Duration" || zone.Name == "TransactionDate" || zone.Name.Contains("Amount")) 
                                zone.CharacterFilters = OcrZoneCharacterFilters.Digit | OcrZoneCharacterFilters.Punctuation;
                            else if (zone.Name == "Calltime")
                                zone.CharacterFilters = OcrZoneCharacterFilters.Digit | OcrZoneCharacterFilters.Punctuation | OcrZoneCharacterFilters.Uppercase;
                            else if (zone.Name == "Callfrom" || zone.Name == "Callto" || zone.Name == "Origination" || zone.Name == "Description" || zone.Name == "TransactionType" || zone.Name == "AccountHolder1")
                                zone.CharacterFilters = OcrZoneCharacterFilters.Digit | OcrZoneCharacterFilters.Punctuation | OcrZoneCharacterFilters.Alpha;
                            else if (zone.Name == "CityState" || zone.Name.Contains("AccountHolder"))
                                zone.CharacterFilters = OcrZoneCharacterFilters.None;
                            else if (zone.Name == "CheckNumber" || zone.Name == "CardNumber" || zone.Name == "AccountNumber" || zone.Name == "RoutingNumber")
                                zone.CharacterFilters = OcrZoneCharacterFilters.Digit;

							// Add zone to page as if it was appled (DG).
                            zone.Bounds = LogicalRectangle.FromRectangle(currentHighlightRect);
                            zone.ZoneType = OcrZoneType.Text;
                            ocrPageProfessional.Zones.Add(zone);

							// If this zone is on the top line, include it in the line zone file to apply on future pages.
                            if (ocrPageProfessional.Zones[0].Bounds.Y >= zone.Bounds.Y - 5)
                            {
                                if (lineOcrPage == null)
                                    saveZoneLine(null, null);

                                lineOcrPage.Zones.Add(zone);
                                lineOcrPage.SaveZones(zoneLineFile);
                            }

                            toolStripButtonApplyZones.Enabled = true;
                            toolStripButtonClearZones.Enabled = true;
                        }
                        else
                        {
                            currentHighlightRect = Leadtools.LeadRect.Empty;
                            viewer.Invalidate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
                viewer.Invalidate();
                zoomComboBox.Enabled = true;
            }
        }

        private void viewer_Paint(object sender, ImageViewerRenderEventArgs e)
        {
            Graphics g = e.PaintEventArgs.Graphics;
            try
            {
				// Draw all the zones that were saved for the current page.
                if (viewer.Image != null && (currentHighlightRect != Leadtools.LeadRect.Empty || ocrPageProfessional.Zones.Count > 0))
                {
                    if (ocrPageProfessional.Zones.Count > 0)
                    {
                        OcrZone zone;
                        for (int x = 0; x < ocrPageProfessional.Zones.Count; x++)
                        {
                            zone = ocrPageProfessional.Zones[x];
                            Leadtools.LeadRect rect = new Leadtools.LeadRect((int)zone.Bounds.Left, (int)zone.Bounds.Top, (int)zone.Bounds.Width, (int)zone.Bounds.Height);
                            Leadtools.LeadRect imageRect = viewer.ConvertRect(null, ImageViewerCoordinateType.Image, ImageViewerCoordinateType.Control, rect);
                            Rectangle drawRect = Rectangle.FromLTRB(imageRect.Left, imageRect.Top, imageRect.Right, imageRect.Bottom);

							// The selected zone is drawn in blue.
                            if (zone.Equals(selectedZone))
                            {
                                g.DrawRectangle(new Pen(Color.Blue), drawRect);
                                g.FillRectangle(new SolidBrush(Color.FromArgb(40, Color.Blue)), drawRect);
                            }
                            else
                            {
                                g.DrawRectangle(new Pen(Color.Orange), drawRect);
                                g.FillRectangle(new SolidBrush(Color.FromArgb(70, Color.Yellow)), drawRect);
                            }

                        }
                    }

					// currentHighlightRect is used for creating new line zones, so draw a rectangle. It will be Empty when applying line zones.
                    if (currentHighlightRect != Leadtools.LeadRect.Empty)
                    {
                        Leadtools.LeadRect imageRect = viewer.ConvertRect(null, ImageViewerCoordinateType.Image, ImageViewerCoordinateType.Control, currentHighlightRect);
                        Rectangle drawRect = Rectangle.FromLTRB(imageRect.Left, imageRect.Top, imageRect.Right, imageRect.Bottom);
                        g.DrawRectangle(new Pen(Color.Orange), drawRect);
                        g.FillRectangle(new SolidBrush(Color.FromArgb(70, Color.Yellow)), drawRect);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButtonRecentBatch(object sender, EventArgs e)
        {
            ToolStripButton button = (ToolStripButton)sender;

			// I: drive must be mapped to \\atr-lsb-lss01\DocuGenie\DGProjects.
			string recentBatchPath = "I:\\" + button.Text.Replace(" ", "\\").Replace("-", "\\");

			if (Directory.Exists(recentBatchPath) && Directory.GetFiles(recentBatchPath, "*.tif").Count > 0)
				openBatch(recentBatchPath);
        }

        private void toolStripButtonZoneLine(object sender, EventArgs e)
        {
            // Load zones for the selected line zone file (e.g. ApplyZone.ozf).
            ToolStripMenuItem zoneLine = (ToolStripMenuItem)sender;
            zoneLineFile = batchPath + "\\" + zoneLine.Text + ".ozf";
            loadZoneLine();
        }

        private void toolStripButtonApplyZones_Click(object sender, EventArgs e)
        {
			// lineOcrPage is required. If it doesn't exist, save the outstanding one that was just drawn.
            if (lineOcrPage == null)
                saveZoneLine(null, null);

			// Load line zones in case they have no zones.
            if (lineOcrPage.Zones.Count == 0 || hadLoadZoneLineError == true) loadZoneLine();
            LogicalRectangle newPageZoneRect, lineZoneRect;
            IOcrPage endCheckOcrPage = null;
            OcrZone newZone, emptyZone;
            double top = 0, left = 0, verticalSpan = 0, lastTop = 0, diff, xDelta = 0;
            int lineCounter = 0, linesPerPage = 100, blankCounter = 0;
			bool performBlankCheck = true;
            Cursor = Cursors.AppStarting;
            Application.DoEvents();

            // contextClickedY will be -1 when user has not right clicked to apply line zones to a specific part of the page.
			// Right click usually means that the user does not want to clear all zones on the page first.
			// But, if no right click, start with a clean slate.
            if (contextClickedY == -1)
                ocrPageProfessional.Zones.Clear();

			// If the expected number of lines per page is not set, or "Apply" (versus Quick Apply) is clicked or the "Apply Once" menu option,
			// set up endCheckOcrPage which will perform a mini-OCR to see if more than 2 blank lines have been reached, usually meaning the end of the page.
            if (toolStripTextBoxLinesPerPage.Text == "" || sender == toolStripButtonApplyZones || sender == quickApplyOnceToolStripMenuItem)
            {
                int pageNumber = int.Parse(toolStripTextBoxPageNumber.Text);
                RasterImage image = codecs.Load(images[pageNumber - 1]);
                endCheckOcrPage = ocrEngineProfessional.CreatePage(image, OcrImageSharingMode.AutoDispose);

                if (sender == quickApplyOnceToolStripMenuItem)
                    linesPerPage = 1;
            }
            else
            {
				// linesPerPage allows an apply that does not check for blanks on every line, and faster as a result.
                linesPerPage = int.Parse(toolStripTextBoxLinesPerPage.Text);
            }

			// Loop through all expected lines, or if set to 100, check every line to see if the end has been reached.
            for (int counter = 0; counter < linesPerPage; counter++)
            {
                zoneCounter = 0;
                left = 0;

				// Loop through each zone in the currently selected line zones to create new zones for the page.
                foreach (OcrZone zone in lineOcrPage.Zones)
                {
                    lineZoneRect = zone.Bounds;
                    newPageZone = new OcrZone();
                    newPageZone.Name = zone.Name;

                    if (sender == quickApplyOnceToolStripMenuItem)
                    {
						// The top starting point is where the user right clicked.
                        if (top == 0)
                        {
                            top = contextClickedY;
							// lastTop is used to measure top differences between line zones
							// as they were originally defined in the .ozf file,
							// so that the actual top used can be adjusted by that amount.
                            lastTop = lineZoneRect.Top;
                        }
                        else
                        {
							// Adjust the top for newPageZone according the line zones in use.
                            diff = lineZoneRect.Top - lastTop;
                            top += diff;
                            lastTop = lineZoneRect.Top;
                        }

						// The left starting point is where the user right clicked or if no right click,
						// where the zone was defined for the line zones.
                        if (left == 0)
                        {
                            left = contextClickedX == -1 ? lineZoneRect.Left : contextClickedX;
							// xDelta is how far over we should be relative to where the user right clicked.
                            xDelta = contextClickedX == -1 ? 0 : contextClickedX - lineZoneRect.Left;
                        }
                        else
                        {
							// Adjust the left for newPageZone according the line zones in use.
                            left = lineZoneRect.Left + xDelta;
                        }
                    }
                    else
                    {
                        if (top == 0)
                        {
							// verticalSpan adusts the top coordinate for the next line by the space between lines
                            // as specified in the Spacing box in the toolbar and the height of lineZoneRect.
                            verticalSpan = (lineZoneRect.Bottom - lineZoneRect.Top) + double.Parse(toolStripComboBoxSpacing.Text);
							// Use contextClickedY if right click occured. If it's a -1, lineZoneRect as it was defined.
                            top = contextClickedY == -1 ? lineZoneRect.Top : contextClickedY;
                            lastTop = lineZoneRect.Top;
                        }
						else
						{
							// Adjust the top for newPageZone according the line zones in use.
							diff = lineZoneRect.Top - lastTop;
							top += diff;
							lastTop = lineZoneRect.Top;
                        }

						// The left starting point is where the user right clicked or if no right click,
						// where the zone was defined for the line zones.
                        if (left == 0)
                        {
                            left = contextClickedX == -1 ? lineZoneRect.Left : contextClickedX;
							// xDelta is how far over we should be relative to where the user right clicked.
                            xDelta = contextClickedX == -1 ? 0 : contextClickedX - lineZoneRect.Left;
                        }
                        else
                        {
							// Adjust the left for newPageZone according the line zones in use.
                            left = lineZoneRect.Left + xDelta;
                        }
                    }

					// Set the coordinates for the new zone for the page.
                    newPageZoneRect = new LogicalRectangle(left, top, lineZoneRect.Width, lineZoneRect.Height, LogicalUnit.Pixel);
                    newPageZone.Bounds = newPageZoneRect;

					// This was used to keep looking down to capture the variable number of lines
					// in a description. It turned out to be too slow to be worth it.
//                    if (hasDescription == true)
//                    {
//                        double nextTop = top + lineZoneRect.Height;
//                        descriptionHeight = lineZoneRect.Height;
//
//                      while (true)
//                        {
//                            emptyZone = new OcrZone();
//                            newPageZoneRect = new LogicalRectangle(left, nextTop, lineZoneRect.Width, lineZoneRect.Height, LogicalUnit.Pixel);
//                            emptyZone.Bounds = newPageZoneRect;
//                            endCheckOcrPage.Zones.Clear();
//                            endCheckOcrPage.Zones.Add(emptyZone);
//                            try
//                            {
//                                endCheckOcrPage.Recognize(null);
//
//                                if (endCheckOcrPage.GetText(0) == "")
//                                {
//                                    descriptionHeight += lineZoneRect.Height / 2;
//                                    nextTop += lineZoneRect.Height + double.Parse(toolStripComboBoxSpacing.Text);
//                                }
//                                else
//                                {
//                                    nextTop += double.Parse(toolStripComboBoxSpacing.Text);
//                                    descriptionHeight += double.Parse(toolStripComboBoxSpacing.Text);
//                                    verticalSpan = descriptionHeight + double.Parse(toolStripComboBoxSpacing.Text);
//                                    break;
//                                }
//                            }
//                            catch
//                            {
//                                break;
//                            }
//                        }
//                    }

					// If "Apply" (not "Quick") was clicked, check for blank lines.
                    if (linesPerPage == 100)
                    {
                        if (performBlankCheck == true)
                        {
                            endCheckOcrPage.Zones.Clear();
                            endCheckOcrPage.Zones.Add(newPageZone);
                            
                            try
                            {
                                endCheckOcrPage.Recognize(null);

                                if (endCheckOcrPage.GetText(0) == "")
                                {
                                    blankCounter += 1;
									// break will stop attempting to create page zones for this line,
									// including the current zone.
                                    break;
                                }
                                else
                                {
									performBlankCheck = false;
                                    blankCounter = 0;
                                }
                            }
                            catch 
                            {
                                blankCounter += 1;
								// break will stop attempting to create page zones for this line,
                                // including the current zone.
                                break;
                            }
                        }
                    }

                    ocrPageProfessional.Zones.Add(newPageZone);

                }  // line zone loop.

				// Give up completely if one blank line was detected. May need adustment.
				if (blankCounter > 0)
                    break;

                top += verticalSpan;

				// When checking for blank lines, keep track of the number of lines per page
				// so that "Lines/page:" in the toolbar can be set automatically.
                if (linesPerPage == 100)
                    lineCounter += 1;
            }

			// Set the number of lines per page found so that "Quick" can now be used.
            if (linesPerPage == 100 && lineCounter > 0 && contextClickedY == -1)
            {
                if (fixedLinespageToolStripMenuItem.Checked == false)
                {
                    if (toolStripTextBoxLinesPerPage.Text != "")
                    {
                        int linesPerPage = int.Parse(toolStripTextBoxLinesPerPage.Text);

						// Do not set "Lines/page:" if the number of lines found is
						// significantly less than the number of lines found before.
                        if (linesPerPage - 10 < lineCounter)
                            toolStripTextBoxLinesPerPage.Text = lineCounter.ToString();
                    }
                    else
                    {
                        // Set "Lines/page:".
                        toolStripTextBoxLinesPerPage.Text = lineCounter.ToString();
                    }
                }

                toolStripButtonQuickApplyZones.Enabled = true;
            }

			// Enable these buttons if zones were created.
            if (ocrPageProfessional.Zones.Count > 0)
            {
                toolStripButtonClearZones.Enabled = true;
                toolStripButtonDeleteLastLine.Enabled = true;
            }

			// Reset contextClickedY.
            contextClickedY = -1;
            Cursor = Cursors.Default;
            viewer.Invalidate();
        }

        private void toolStripButtonClearZones_Click(object sender, EventArgs e)
        {
			// Clear, reset, disable and delete as appropriate for when there are no zones on the page.
            ocrPageProfessional.Zones.Clear();
            toolStripButtonClearZones.Enabled = false;
            selectedZone = new OcrZone();
            currentHighlightRect = Leadtools.LeadRect.Empty;
            toolStripButtonDeleteLastLine.Enabled = false;
            int pageNumber = int.Parse(toolStripTextBoxPageNumber.Text);
            string pageImageFileRoot = images[pageNumber - 1];
            pageImageFileRoot = pageImageFileRoot.Substring(pageImageFileRoot.LastIndexOf("\\") + 1);
            pageImageFileRoot = pageImageFileRoot.Substring(0, pageImageFileRoot.LastIndexOf("."));
            string pageZonesFile = batchPath.Replace(":", ":\\") + "\\" + pageImageFileRoot + ".ozf";

            if (File.Exists(pageZonesFile))
                File.Delete(pageZonesFile);

            viewer.Invalidate();
        }

        private void toolStripButtonDeleteLastLine_Click(object sender, EventArgs e)
        {
			// If there is at least one line on the page.
            if (ocrPageProfessional.Zones.Count > 4)
            {
				// Remove the last zonesPerLine (5) zones on the page.
                for (int zoneCounter = 0; zoneCounter < zonesPerLine; zoneCounter++)
                {
                    ocrPageProfessional.Zones.RemoveAt(ocrPageProfessional.Zones.Count - 1);
                }
                viewer.Invalidate();
            }
        }

        private void openBatch(string path)
        {
            if (!Directory.Exists(path))
            {
                Messager.ShowInformation(this, "Batch path " + path + " does not exist.");
                return;
            }

			// Set openingBath flag to true to prevent setting of default spacing to cause
			// an automatic apply or setting the page number to refresh the page.
            openingBatch = true;
            batchPath = path;
            zoneLineFile = "";
            images = Directory.GetFiles(batchPath, "*.tif");

            if (images.Length == 0)
            {
                Messager.ShowInformation(this, "There are no TIF files in this batch folder.");
                return;
            }

            batchTableName = "Phone";
            batchDateField = "Calldate";

			// Get the rest of the batch path after I:\
            if (path.Length > 4)
            {
                if (path.Substring(2, 1) == "\\")
                    batchName = path.Substring(3);
                else
                    batchName = path.Substring(2);
            }

			// Parse the folder bath into a standard batch name, depending on the number of subfolders.
            if (batchName.Split('\\').Length == 3)
                batchName = batchName.Split('\\')[0] + " " + batchName.Split('\\')[1] + "-" + batchName.Split('\\')[2];
            else if (batchName.Split('\\').Length == 2)
                batchName = batchName.Split('\\')[0] + " " + batchName.Split('\\')[1];

            this.Text = "CamGenie " + versionNum + " - " + batchName;
            imageFilePaths = Directory.GetFiles(batchPath, "*.tif");
            toolStripLabelPageTotal.Text = "of " + images.Length.ToString();
            toolStripTextBoxPageNumber.Text = "1";
            currentPageNumber = 1;
			// Clear zones from a prior batch.
			if (ocrPageProfessional != null) ocrPageProfessional.Zones.Clear();
			// Load the first page (TIF) in the batch.
			LoadImage();

			// Set the default line zones file if it exists.
            if (File.Exists(path + "\\ApplyZones.ozf"))
            {
                zoneLineFile = path + "\\ApplyZones.ozf";
            }
            else
            {
                lineOcrPage = null;
                toolStripButtonApplyZones.Enabled = false;
            }

            string batchConfigFile = batchPath + "\\batch.config";

            if (File.Exists(batchConfigFile))
            {
                string line;

                using (StreamReader sr = new StreamReader(batchConfigFile))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
						// Go to the last page viewed.
                        if (line.StartsWith("LastPage"))
                        {
                            string lastPage = line.Split('=')[1];
                            
                            if (int.Parse(lastPage) <= images.Length)
                            {
                                toolStripTextBoxPageNumber.Text = lastPage;
                                LoadImage();
                            }
                        }
						// Set the default lines per page last used.
                        else if (line.StartsWith("LinesPerPage"))
                        {
                            string linesPerPage = line.Split('=')[1];
                            toolStripTextBoxLinesPerPage.Text = linesPerPage;
                        }
						// Set the appropriate batch type.
                        else if (line.StartsWith("BatchType"))
                        {
                            batchType = line.Split('=')[1];

                            if (batchType == "BankStatement")
                            {
                                batchTableName = "BankStatement";
                                batchDateField = "TransactionDate";
                            }
                        }
						// Set the default line zones file to the last one used.
                        else if (line.StartsWith("LastApplyZoneFile"))
                        {
							//  If set to blank.
                            if (line.Split('=')[1] == "")
                                zoneLineFile = path + "\\ApplyZones.ozf";
                            else
                                zoneLineFile = path + "\\" + line.Split('=')[1];
                        }
                    }
                }
            }
            else
            {
				// Create a blank config file.
                StreamWriter sw = new StreamWriter(batchConfigFile, false);
                sw.Close();
            }

            loadZoneLine();
			// Allow user to set batch properties manually.
            batchPropertiesToolStripMenuItem.Enabled = true;
            OleDbConnection oleDbConnection = new OleDbConnection(connectionString);
            oleDbConnection.Open();
            string sql = "select count(*) from " + batchTableName + " where Batch = '" + batchName + "'";
            OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
            DataTable dataTable = new DataTable();
            oleDbDataAdapter.Fill(dataTable);

			// Records will exist in batchTableName if OCR was run. Only then is Main QC available.
            ocrQCToolStripMenuItem.Enabled = dataTable.Rows[0][0].ToString() != "0";
			// Disable "Apply" and "Quick Apply" buttons if OCR was already executed.
            toolStripButtonApplyZones.Enabled = dataTable.Rows[0][0].ToString() == "0";
            toolStripButtonQuickApplyZones.Enabled = dataTable.Rows[0][0].ToString() == "0";

            sql = "select count(*) from " + batchTableName + " where Batch = '" + batchName + "' and " + batchDateField + " is not null";
            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
            dataTable = new DataTable();
            oleDbDataAdapter.Fill(dataTable);

			// Final QC is only available if date field is populated which is done at the end of Main QC.
            finalQCToolStripMenuItem.Enabled = dataTable.Rows[0][0].ToString() != "0";

            oleDbConnection.Close();
            saveFileDialogZones.InitialDirectory = batchPath;

			// Rearrange the recent batch menu batch names so most recent is first.
            if (Properties.Settings.Default.RecentBatch1 != "" && Properties.Settings.Default.RecentBatch2 == "")
            {
                Properties.Settings.Default.RecentBatch2 = Properties.Settings.Default.RecentBatch1;
            }
            else if (Properties.Settings.Default.RecentBatch3 == "")
            {
                Properties.Settings.Default.RecentBatch3 = Properties.Settings.Default.RecentBatch2;
                Properties.Settings.Default.RecentBatch2 = Properties.Settings.Default.RecentBatch1;
            }

            Properties.Settings.Default.RecentBatch1 = batchName;
            Properties.Settings.Default.Save();

			// Get all zone line files that exist and default menus to invisible.
            string[] zoneLineFiles = Directory.GetFiles(batchPath, "Apply*.ozf");
			ToolStripMenuItem toolStripMenuItem;
			toolStripSeparatorZoneLine.Visible = false;

			for (int menuCounter; menuCounter < 16; menuCounter++)
			{
                toolStripMenuItem = zonesToolStripMenuItem.DropDownItems.Find("zoneLine" + menuCounter.ToString() + "toolStripMenuItem", true)[0] as ToolStripMenuItem;
                toolStripMenuItem.Visible = false;
            }

            if (zoneLineFiles.Length > 0)
            {
                int menuCounter = 1;
                toolStripSeparatorZoneLine.Visible = true;

				// Set menus for all available zone line files, up to 15;
                foreach (string zoneLineFile in zoneLineFiles)
                {
					toolStripMenuItem = zonesToolStripMenuItem.DropDownItems.Find("zoneLine" + menuCounter.ToString() + "toolStripMenuItem", true)[0] as ToolStripMenuItem;
					toolStripMenuItem.Text = zoneLineFile.Replace(batchPath + "\\", "").Replace(".ozf", "");
					toolStripMenuItem.Visible = true;
                    menuCounter++;

					if (menuCounter == 16)
						break;
                }
            }
            
            openingBatch = false;
        }

        private void toolStripButtonNextPage_Click(object sender, EventArgs e)
        {
			// Increment the page number and load the next page if not on the last.
            int pageNumber = int.Parse(toolStripTextBoxPageNumber.Text);

            if (pageNumber < images.Length)
            {
                pageNumber += 1;
                toolStripTextBoxPageNumber.Text = pageNumber.ToString();
                LoadImage();
            }
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
				// Enter key advances to the next page, but only if one of these controls has focus.
                if (this.ActiveControl == viewer || this.ActiveControl.GetType().Name == "ToolStripComboBoxControl" || this.ActiveControl.GetType().Name == "ToolStripTextBoxControl")
                {
					// If the page number text box has focus, go to that page.
                    if (this.ActiveControl.Text == toolStripTextBoxPageNumber.Text)
                    {
                        int pageNumber = int.Parse(toolStripTextBoxPageNumber.Text);

                        if (pageNumber < images.Length)
                        {
                            LoadImage();
                        }
                    }
                    else
                    {
						// Otherwise, go to the next page.
                        toolStripButtonNextPage_Click(null, null);
                    }
                }
                else
                {
					// Go to the page number page if focus is not on the viewer, a combo box or text box.
                    int pageNumber = int.Parse(toolStripTextBoxPageNumber.Text);

                    if (pageNumber < images.Length)
                    {
                        LoadImage();
                    }
                }
            }
			// Apply zones when Ctrl+Enter is pressed.
            else if (e.KeyChar == (char)10 && lineOcrPage != null)
            {
                toolStripButtonApplyZones_Click(toolStripButtonApplyZones, null);
            }
			// Zoom out when number pad minus is pressed.
            else if (e.KeyChar == (char)45)
            {
                double scaleFactor = viewer.ScaleFactor;
                scaleFactor /= 1.1f;
                Leadtools.LeadPoint origin = new Leadtools.LeadPoint(100, 1);

				// Minimum zoom level.
                if (scaleFactor < 0.009)
                    scaleFactor = 0.009F;

				// Zoom if scale factor has changed.
                if (scaleFactor != viewer.ScaleFactor)
                    viewer.Zoom(ControlSizeMode.None, scaleFactor, origin);
            }
			// Zoom in when number pad plus is pressed.
			else if (e.KeyChar == (char)43)
            {
                double scaleFactor = viewer.ScaleFactor;
                scaleFactor *= 1.1f;
                Leadtools.LeadPoint origin = new Leadtools.LeadPoint(100, 1);

				// Maximum zoom level is 11x.
                if (scaleFactor > 11)
                    scaleFactor = 11;

				// Zoom if scale factor has changed.
                if (scaleFactor != viewer.ScaleFactor)
                    viewer.Zoom(ControlSizeMode.None, scaleFactor, origin);
            }
            // Return to Fit zoom level when Esc is pressed.
            else if (e.KeyChar == (char)27)
            {
                if (zoomComboBox.SelectedIndex == 0)
                    zoomComboBox_SelectedIndexChanged(null, null);
                else
                    zoomComboBox.SelectedIndex = 0;

            }
        }

        private void toolbar_KeyPress(object sender, KeyPressEventArgs e)
        {
			// Go to next page if Enter is pressed on the toolbar.
            if (e.KeyChar == (char)13)
            {
                toolStripButtonNextPage_Click(null, null);
            }
        }

        private void toolStripButtonPreviousPage_Click(object sender, EventArgs e)
        {
            int pageNumber = int.Parse(toolStripTextBoxPageNumber.Text);

			// Go to previous page if current page is not page 1.
            if (pageNumber > 1)
            {
                pageNumber -= 1;
                toolStripTextBoxPageNumber.Text = pageNumber.ToString();
                LoadImage();
            }
        }

        private void toolStripButtonFirstPage_Click(object sender, EventArgs e)
        {
            toolStripTextBoxPageNumber.Text = "1";
            LoadImage();
        }

        private void toolStripButtonLastPage_Click(object sender, EventArgs e)
        {
            toolStripTextBoxPageNumber.Text = images.Length.ToString();
            LoadImage();
        }

        private void openFileDialogZones_FileOk(object sender, CancelEventArgs e)
        {
            // Load zones for selected zone line file.
            if (openFileDialogZones.FileName != "")
            {
                zoneLineFile = openFileDialogZones.FileName;
                loadZoneLine();
            }
        }

        private void loadZoneLine()
        {
			// Load zones for zoneLineFile into the lineOcrPage object so it case be used when applying.
            hadLoadZoneLineError = false;
            int pageIndex = toolStripTextBoxPageNumber.Text == "" ? 0 : int.Parse(toolStripTextBoxPageNumber.Text) - 1;
            RasterImage image = codecs.Load(images[pageIndex]);
            lineOcrPage = ocrEngineProfessional.CreatePage(image, OcrImageSharingMode.AutoDispose);

            try { 
                lineOcrPage.LoadZones(zoneLineFile);
            }
            catch 
            {
				// Try other zone line files if first failed to load, keeping track of ones that were attempted.
                Hashtable attemptedZoneFiles = new Hashtable();
                attemptedZoneFiles.Add(zoneLineFile, zoneLineFile);
                string[] zoneFiles = Directory.GetFiles(batchPath, "*.ozf");
                foreach (string zoneFile in zoneFiles)
                {
                    if (!attemptedZoneFiles.ContainsKey(zoneFile))
                    {
                        zoneLineFile = zoneFile;
                        attemptedZoneFiles.Add(zoneLineFile, zoneLineFile);
                        try
                        {
                            lineOcrPage.LoadZones(zoneLineFile);
                            break;
                        }
                        catch
                        {
                        }
                    }
                }

				// Loading zones will be tried again when apply is done.
                if (lineOcrPage.Zones.Count == 0)
                    hadLoadZoneLineError = true;
            }

            zonesPerLine = lineOcrPage.Zones.Count;
            toolStripButtonApplyZones.Enabled = true;
			// Reset lines per page when a new zone line file is in use.
            toolStripTextBoxLinesPerPage.Text = "";
            viewer.Invalidate();

            if (batchPath != "")
            {
                string batchConfigFile = batchPath + "\\batch.config", batchTempFile = batchPath + "\\batch.tmp";
	            string fileName = zoneLineFile.Substring(zoneLineFile.LastIndexOf("\\") + 1);
                string spaceValue = "";
                string line;
                bool foundLastApplyZoneFile = false;

				// If the batch config has a line for this zone line, grab the spacing value and use it.
                if (File.Exists(batchConfigFile))
                {
                    using (StreamReader sr = new StreamReader(batchConfigFile))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Contains(zoneLineFile))
                            {
                                spaceValue = line.Split('=')[1];
                                break;
                            }
                        }
                    }

                    if (spaceValue != "")
                        toolStripComboBoxSpacing.SelectedIndex = toolStripComboBoxSpacing.FindStringExact(spaceValue);
                }

				// Update the existing or add the last used zone line file to batch config.
				StreamWriter sw = new StreamWriter(batchTempFile, false);

                using (StreamReader sr = new StreamReader(batchConfigFile))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("LastApplyZoneFile"))
                        {
                            sw.WriteLine("LastApplyZoneFile=" + fileName);
                            foundLastApplyZoneFile = true;
                        }
                        else
                            sw.WriteLine(line);
                    }
                }

                if (foundLastApplyZoneFile == false)
                {
                    sw.WriteLine("LastApplyZoneFile=" + fileName);
                }

                sw.Close();
				// Replace the batch config file.
                File.Delete(batchConfigFile);
                File.Move(batchTempFile, batchConfigFile);
            }

			// Display the zone line file currently in use.
            toolStripStatusLabelApplyZone.Text = zoneLineFile.Substring(zoneLineFile.LastIndexOf("\\") + 1);
        }

        private void saveFileDialogZones_FileOk(object sender, CancelEventArgs e)
        {
            if (saveFileDialogZones.FileName != "")
            {
				// Save the zones on the current page to the selected zone line file, or create one.
                zoneLineFile = saveFileDialogZones.FileName;
                string menuText = zoneLineFile.Replace(batchPath + "\\", "").Replace(".ozf", "");
                if (File.Exists(zoneLineFile)) File.Delete(zoneLineFile);
                ocrPageProfessional.SaveZones(zoneLineFile);
                toolStripStatusLabelApplyZone.Text = zoneLineFile.Substring(zoneLineFile.LastIndexOf("\\") + 1);
                ToolStripMenuItem toolStripMenuItem;

				// Add to the first available zone line menu item if it's not already among the visible ones.
                for (int index = 1; index < 16; index++)
                {
                    toolStripMenuItem = zonesToolStripMenuItem.DropDownItems.Find("zoneLine" + index.ToString() + "toolStripMenuItem", true)[0] as ToolStripMenuItem;

                    if (toolStripMenuItem.Visible == true)
                    {
                        if (toolStripMenuItem.Text == menuText)
                            break;
                    }
                    else
                    {
                        toolStripMenuItem.Text = menuText;
                        toolStripMenuItem.Visible = true;
                        break;
                    }
                }
            }
        }

        private void toolStripButtonClearLinesPerPage_Click(object sender, EventArgs e)
        {
            toolStripTextBoxLinesPerPage.Text = "";
        }

        private void saveZonesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialogZones.ShowDialog(this);
        }

        private void loadZonesToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// Show all zone line files in the current batch path.
            openFileDialogZones.InitialDirectory = batchPath.Replace(":", ":\\");
            openFileDialogZones.ShowDialog(this);
        }

        private void toolStripButtonQuickApplyZones_Click(object sender, EventArgs e)
        {
            toolStripButtonApplyZones_Click(toolStripButtonQuickApplyZones, null);
        }

        private void openBatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// Display select batch dialog and open selected batch.
            SelectBatchFolder selectBatchFolder = new SelectBatchFolder();

            if (selectBatchFolder.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                openBatch(selectBatchFolder.selectedPath);
            }
        }

        private void applyToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// Right click apply. Save coordinates in buffer that can be changed.
            contextClickedY = clickedY;
            contextClickedX = clickedX;
            toolStripButtonApplyZones_Click(toolStripButtonApplyZones, null);
        }

        private void toolStripTextBoxPageNumber_MouseUp(object sender, MouseEventArgs e)
        {
			// Select all in the text box when it is clicked.
            SendKeys.SendWait("^A");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void viewer_MouseDown(object sender, MouseEventArgs e)
        {
			// Save mouse coordinates for right click apply.
            Leadtools.LeadPoint mousePoint = new Leadtools.LeadPoint(e.Location.X, e.Location.Y);
            Leadtools.LeadPoint point = viewer.ConvertPoint(null, ImageViewerCoordinateType.Control, ImageViewerCoordinateType.Image, mousePoint);
            clickedY = point.Y;
            clickedX = point.X;

            if (ocrPageProfessional != null && ocrPageProfessional.Zones != null && ocrPageProfessional.Zones.Count > 0)
            {
                OcrZone foundZone = new OcrZone(), zone;

				// Find the zone where the mouse clicked comparing zone coordinates with mouse coordinates.
                for (int x = 0; x < ocrPageProfessional.Zones.Count; x++)
                {
                    zone = ocrPageProfessional.Zones[x];

                    if (clickedY > zone.Bounds.Top && clickedY < zone.Bounds.Bottom && clickedX > zone.Bounds.Left && clickedX < zone.Bounds.Right)
                    {
                        foundZone = zone;
                        currentHighlightRect = Leadtools.LeadRect.Empty;
                        zoneIndex = x;
                        break;
                    }
                }

                selectedZone = foundZone;

				// Display zone info in the status bar.
                if (selectedZone.Name == null || selectedZone.Name.Trim() == "")
                    toolStripStatusLabelZoneName.Text = "";
                else
                    toolStripStatusLabelZoneName.Text = selectedZone.Name + ", ID " + selectedZone.Id.ToString() + ", T:" + selectedZone.Bounds.Top.ToString() + ", B:" + selectedZone.Bounds.Bottom.ToString() + ", L:" + selectedZone.Bounds.Left.ToString() + ", R:" + selectedZone.Bounds.Right.ToString();
            }

            viewer.Invalidate();
        }

        private void toolStripComboBoxSpacing_SelectedIndexChanged(object sender, EventArgs e)
        {
			// Save the selected spacing to the batch config for the current zone line file.
            if (toolStripComboBoxSpacing.SelectedIndex > -1 && batchPath != "" && openingBatch == false)
            {
                string batchConfigFile = batchPath + "\\batch.config", batchTempFile = batchPath + "\\batch.tmp";
                string spaceValue = toolStripComboBoxSpacing.Text;
                string line;
                bool foundLineSpace = false;

                if (File.Exists(batchConfigFile))
                {
                    StreamWriter sw = new StreamWriter(batchTempFile, false);

                    using (StreamReader sr = new StreamReader(batchConfigFile))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Contains(zoneLineFile))
                            {
                                sw.WriteLine(zoneLineFile + "-LineSpace=" + spaceValue);
                                foundLineSpace = true;
                            }
                            else
                                sw.WriteLine(line);
                        }
                    }

                    if (foundLineSpace == false)
                    {
                        sw.WriteLine(zoneLineFile + "-LineSpace=" + spaceValue);
                    }

                    sw.Close();
                    File.Delete(batchConfigFile);
                    File.Move(batchTempFile, batchConfigFile);
                }
                else
                {
                    StreamWriter sw = new StreamWriter(batchConfigFile, false);
                    sw.WriteLine(zoneLineFile + "-LineSpace=" + spaceValue);
                    sw.Close();
                }
            }
        }

		// Automatically fade the help display for key legend to manipulate zone size.
        private void timerFade_Tick(object sender, EventArgs e)
        {
            if (fadeCounter > 40)
            {
                timerFade.Interval = 10;

                if (fadeCounter > 255)
                {
                    timerFade.Enabled = false;
                    timerFade.Interval = 1000;
                    pictureBoxAdjust.Visible = false;
                    labelLeftArrow.Visible = false;
                    labelMoveLeft.Visible = false;
                    labelRightArrow.Visible = false;
                    labelMoveRight.Visible = false;
                    labelMoveDown.Visible = false;
                    labelDownArrow.Visible = false;
                    labelUpArrow.Visible = false;
                    labelMoveUp.Visible = false;
                    labelShiftDown.Visible = false;
                    labelShiftDownArrow.Visible = false;
                    labelMoveBottomDown.Visible = false;
                    labelShiftUp.Visible = false;
                    labelShiftUpArrow.Visible = false;
                    labelMoveTopUp.Visible = false;
                    labelShiftRight.Visible = false;
                    labelShiftRightArrow.Visible = false;
                    labelMoveRightToRight.Visible = false;
                    labelShiftLeft.Visible = false;
                    labelShiftLeftArrow.Visible = false;
                    labelMoveLeftToLeft.Visible = false;
                    labelCtrlLeft.Visible = false;
                    labelCtrlLeftArrow.Visible = false;
                    labelMoveLeftToRight.Visible = false;
                    labelCtrlRight.Visible = false;
                    labelCtrlRightArrow.Visible = false;
                    labelMoveRightToLeft.Visible = false;
                    labelCtrlDown.Visible = false;
                    labelCtrlDownArrow.Visible = false;
                    labelMoveBottomUp.Visible = false;
                    labelCtrlUp.Visible = false;
                    labelCtrlUpArrow.Visible = false;
                    labelMoveTopDown.Visible = false;
                    SetAdjustColors(true, Color.FromArgb(255, 196, 196, 196));
                    SetAdjustColors(false, Color.FromArgb(1, 0, 0, 0));
                }
                else
                {
                    SetAdjustColors(true, Color.FromArgb(255 - (fadeCounter - 10), 196, 196, 196));
                    SetAdjustColors(false, Color.FromArgb(1, fadeCounter - 10, fadeCounter - 10, fadeCounter - 10));
                }
            }

            fadeCounter += 5;
        }

		// Set gradually change colors to automatically fade the help display for the zone key legend.
        private void SetAdjustColors(bool isBackColor, Color color)
        {
            if (isBackColor) pictureBoxAdjust.BackColor = color;
            if (isBackColor) labelLeftArrow.BackColor = color; else labelLeftArrow.ForeColor = color;
            if (isBackColor) labelMoveLeft.BackColor = color; else labelMoveLeft.ForeColor = color;
            if (isBackColor) labelRightArrow.BackColor = color; else labelRightArrow.ForeColor = color;
            if (isBackColor) labelMoveRight.BackColor = color; else labelMoveRight.ForeColor = color;
            if (isBackColor) labelDownArrow.BackColor = color; else labelDownArrow.ForeColor = color;
            if (isBackColor) labelMoveDown.BackColor = color; else labelMoveDown.ForeColor = color;
            if (isBackColor) labelUpArrow.BackColor = color; else labelUpArrow.ForeColor = color;
            if (isBackColor) labelMoveUp.BackColor = color; else labelMoveUp.ForeColor = color;
            if (isBackColor) labelShiftDown.BackColor = color; else labelShiftDown.ForeColor = color;
            if (isBackColor) labelShiftDownArrow.BackColor = color; else labelShiftDownArrow.ForeColor = color;
            if (isBackColor) labelMoveBottomDown.BackColor = color; else labelMoveBottomDown.ForeColor = color;
            if (isBackColor) labelShiftRight.BackColor = color; else labelShiftRight.ForeColor = color;
            if (isBackColor) labelShiftRightArrow.BackColor = color; else labelShiftRightArrow.ForeColor = color;
            if (isBackColor) labelMoveRightToRight.BackColor = color; else labelMoveRightToRight.ForeColor = color;
            if (isBackColor) labelShiftLeft.BackColor = color; else labelShiftLeft.ForeColor = color;
            if (isBackColor) labelShiftLeftArrow.BackColor = color; else labelShiftLeftArrow.ForeColor = color;
            if (isBackColor) labelMoveLeftToLeft.BackColor = color; else labelMoveLeftToLeft.ForeColor = color;
            if (isBackColor) labelShiftUp.BackColor = color; else labelShiftUp.ForeColor = color;
            if (isBackColor) labelShiftUpArrow.BackColor = color; else labelShiftUpArrow.ForeColor = color;
            if (isBackColor) labelMoveTopUp.BackColor = color; else labelMoveTopUp.ForeColor = color;
            if (isBackColor) labelCtrlLeft.BackColor = color; else labelCtrlLeft.ForeColor = color;
            if (isBackColor) labelCtrlLeftArrow.BackColor = color; else labelCtrlLeftArrow.ForeColor = color;
            if (isBackColor) labelMoveLeftToRight.BackColor = color; else labelMoveLeftToRight.ForeColor = color;
            if (isBackColor) labelCtrlRight.BackColor = color; else labelCtrlRight.ForeColor = color;
            if (isBackColor) labelCtrlRightArrow.BackColor = color; else labelCtrlRightArrow.ForeColor = color;
            if (isBackColor) labelMoveRightToLeft.BackColor = color; else labelMoveRightToLeft.ForeColor = color;
            if (isBackColor) labelCtrlDown.BackColor = color; else labelCtrlDown.ForeColor = color;
            if (isBackColor) labelCtrlDownArrow.BackColor = color; else labelCtrlDownArrow.ForeColor = color;
            if (isBackColor) labelMoveBottomUp.BackColor = color; else labelMoveBottomUp.ForeColor = color;
            if (isBackColor) labelCtrlUp.BackColor = color; else labelCtrlUp.ForeColor = color;
            if (isBackColor) labelCtrlUpArrow.BackColor = color; else labelCtrlUpArrow.ForeColor = color;
            if (isBackColor) labelMoveTopDown.BackColor = color; else labelMoveTopDown.ForeColor = color;
        }

        private void pictureBoxAdjust_Paint(object sender, PaintEventArgs e)
        {

        }

		// Display zone key legend when "Adjust" context menu is selected.
        private void adjustToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBoxAdjust.Height = 220;
            pictureBoxAdjust.Visible = true;
            labelLeftArrow.Left = pictureBoxAdjust.Left + 45;
            labelLeftArrow.Top = pictureBoxAdjust.Top + 15;
            labelLeftArrow.Visible = true;
            labelMoveLeft.Left = labelLeftArrow.Left + 25;
            labelMoveLeft.Top = labelLeftArrow.Top;
            labelMoveLeft.Visible = true;

            labelRightArrow.Left = labelLeftArrow.Left;
            labelRightArrow.Top = labelLeftArrow.Top + 15;
            labelRightArrow.Visible = true;
            labelMoveRight.Left = labelMoveLeft.Left;
            labelMoveRight.Top = labelMoveLeft.Top + 15;
            labelMoveRight.Visible = true;

            labelDownArrow.Left = labelLeftArrow.Left;
            labelDownArrow.Top = labelRightArrow.Top + 15;
            labelDownArrow.Visible = true;
            labelMoveDown.Left = labelMoveLeft.Left;
            labelMoveDown.Top = labelMoveRight.Top + 15;
            labelMoveDown.Visible = true;

            labelUpArrow.Left = labelLeftArrow.Left;
            labelUpArrow.Top = labelDownArrow.Top + 15;
            labelUpArrow.Visible = true;
            labelMoveUp.Left = labelMoveLeft.Left;
            labelMoveUp.Top = labelMoveDown.Top + 15;
            labelMoveUp.Visible = true;

            labelShiftLeft.Left = pictureBoxAdjust.Left + 16;
            labelShiftLeft.Top = labelUpArrow.Top + 15;
            labelShiftLeft.Visible = true;
            labelShiftLeftArrow.Left = labelLeftArrow.Left;
            labelShiftLeftArrow.Top = labelUpArrow.Top + 15;
            labelShiftLeftArrow.Visible = true;
            labelMoveLeftToLeft.Left = labelMoveLeft.Left;
            labelMoveLeftToLeft.Top = labelUpArrow.Top + 15;
            labelMoveLeftToLeft.Visible = true;

            labelShiftRight.Left = pictureBoxAdjust.Left + 16;
            labelShiftRight.Top = labelShiftLeftArrow.Top + 15;
            labelShiftRight.Visible = true;
            labelShiftRightArrow.Left = labelLeftArrow.Left;
            labelShiftRightArrow.Top = labelShiftLeftArrow.Top + 15;
            labelShiftRightArrow.Visible = true;
            labelMoveRightToRight.Left = labelMoveLeft.Left;
            labelMoveRightToRight.Top = labelShiftLeftArrow.Top + 15;
            labelMoveRightToRight.Visible = true;

            labelShiftDown.Left = pictureBoxAdjust.Left + 16;
            labelShiftDown.Top = labelShiftRightArrow.Top + 15;
            labelShiftDown.Visible = true;
            labelShiftDownArrow.Left = labelLeftArrow.Left;
            labelShiftDownArrow.Top = labelShiftRightArrow.Top + 15;
            labelShiftDownArrow.Visible = true;
            labelMoveBottomDown.Left = labelMoveLeft.Left;
            labelMoveBottomDown.Top = labelShiftRightArrow.Top + 15;
            labelMoveBottomDown.Visible = true;

            labelShiftUp.Left = pictureBoxAdjust.Left + 16;
            labelShiftUp.Top = labelShiftDownArrow.Top + 15;
            labelShiftUp.Visible = true;
            labelShiftUpArrow.Left = labelLeftArrow.Left;
            labelShiftUpArrow.Top = labelShiftDownArrow.Top + 15;
            labelShiftUpArrow.Visible = true;
            labelMoveTopUp.Left = labelMoveLeft.Left;
            labelMoveTopUp.Top = labelShiftDownArrow.Top + 15;
            labelMoveTopUp.Visible = true;

            labelCtrlLeft.Left = pictureBoxAdjust.Left + 16;
            labelCtrlLeft.Top = labelShiftUpArrow.Top + 15;
            labelCtrlLeft.Visible = true;
            labelCtrlLeftArrow.Left = labelLeftArrow.Left;
            labelCtrlLeftArrow.Top = labelShiftUpArrow.Top + 15;
            labelCtrlLeftArrow.Visible = true;
            labelMoveLeftToRight.Left = labelMoveLeft.Left;
            labelMoveLeftToRight.Top = labelShiftUpArrow.Top + 15;
            labelMoveLeftToRight.Visible = true;

            labelCtrlRight.Left = pictureBoxAdjust.Left + 16;
            labelCtrlRight.Top = labelCtrlLeftArrow.Top + 15;
            labelCtrlRight.Visible = true;
            labelCtrlRightArrow.Left = labelLeftArrow.Left;
            labelCtrlRightArrow.Top = labelCtrlLeftArrow.Top + 15;
            labelCtrlRightArrow.Visible = true;
            labelMoveRightToLeft.Left = labelMoveLeft.Left;
            labelMoveRightToLeft.Top = labelCtrlLeftArrow.Top + 15;
            labelMoveRightToLeft.Visible = true;

            labelCtrlDown.Left = pictureBoxAdjust.Left + 16;
            labelCtrlDown.Top = labelCtrlRightArrow.Top + 15;
            labelCtrlDown.Visible = true;
            labelCtrlDownArrow.Left = labelLeftArrow.Left;
            labelCtrlDownArrow.Top = labelCtrlRightArrow.Top + 15;
            labelCtrlDownArrow.Visible = true;
            labelMoveBottomUp.Left = labelMoveLeft.Left;
            labelMoveBottomUp.Top = labelCtrlRightArrow.Top + 15;
            labelMoveBottomUp.Visible = true;

            labelCtrlUp.Left = pictureBoxAdjust.Left + 16;
            labelCtrlUp.Top = labelCtrlDownArrow.Top + 15;
            labelCtrlUp.Visible = true;
            labelCtrlUpArrow.Left = labelLeftArrow.Left;
            labelCtrlUpArrow.Top = labelCtrlDownArrow.Top + 15;
            labelCtrlUpArrow.Visible = true;
            labelMoveTopDown.Left = labelMoveLeft.Left;
            labelMoveTopDown.Top = labelCtrlDownArrow.Top + 15;
            labelMoveTopDown.Visible = true;

            fadeCounter = 0;
            timerFade.Enabled = true;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
			// Move or size zones based on key combination used.
            if (selectedZone.Bounds.Right > selectedZone.Bounds.Left && e.KeyValue > 20)
            {
                OcrZone newZone = selectedZone;
				// Remove and re-add zone with new location or size.
                ocrPageProfessional.Zones.Remove(selectedZone);
                                
                bool ignoreKeystroke = false;

                if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Down)
                    newZone.Bounds = new LogicalRectangle(selectedZone.Bounds.Left, selectedZone.Bounds.Top, selectedZone.Bounds.Width, selectedZone.Bounds.Height + 1, LogicalUnit.Pixel);
                else if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Up)
                    newZone.Bounds = new LogicalRectangle(selectedZone.Bounds.Left, selectedZone.Bounds.Top - 1, selectedZone.Bounds.Width, selectedZone.Bounds.Height + 1, LogicalUnit.Pixel);
                else if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Left)
                    newZone.Bounds = new LogicalRectangle(selectedZone.Bounds.Left - 1, selectedZone.Bounds.Top, selectedZone.Bounds.Width + 1, selectedZone.Bounds.Height, LogicalUnit.Pixel);
                else if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Right)
                    newZone.Bounds = new LogicalRectangle(selectedZone.Bounds.Left, selectedZone.Bounds.Top, selectedZone.Bounds.Width + 1, selectedZone.Bounds.Height, LogicalUnit.Pixel);
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Down)
                    newZone.Bounds = new LogicalRectangle(selectedZone.Bounds.Left, selectedZone.Bounds.Top, selectedZone.Bounds.Width, selectedZone.Bounds.Height - 1, LogicalUnit.Pixel);
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Up)
                    newZone.Bounds = new LogicalRectangle(selectedZone.Bounds.Left, selectedZone.Bounds.Top + 1, selectedZone.Bounds.Width, selectedZone.Bounds.Height - 1, LogicalUnit.Pixel);
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Left)
                    newZone.Bounds = new LogicalRectangle(selectedZone.Bounds.Left + 1, selectedZone.Bounds.Top, selectedZone.Bounds.Width - 1, selectedZone.Bounds.Height, LogicalUnit.Pixel);
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Right)
                    newZone.Bounds = new LogicalRectangle(selectedZone.Bounds.Left, selectedZone.Bounds.Top, selectedZone.Bounds.Width - 1, selectedZone.Bounds.Height, LogicalUnit.Pixel);
                else if (e.KeyCode == Keys.Down)
                    newZone.Bounds = new LogicalRectangle(selectedZone.Bounds.Left, selectedZone.Bounds.Top + 1, selectedZone.Bounds.Width, selectedZone.Bounds.Height, LogicalUnit.Pixel);
                else if (e.KeyCode == Keys.Up)
                    newZone.Bounds = new LogicalRectangle(selectedZone.Bounds.Left, selectedZone.Bounds.Top - 1, selectedZone.Bounds.Width, selectedZone.Bounds.Height, LogicalUnit.Pixel);
                else if (e.KeyCode == Keys.Left)
                    newZone.Bounds = new LogicalRectangle(selectedZone.Bounds.Left - 1, selectedZone.Bounds.Top, selectedZone.Bounds.Width, selectedZone.Bounds.Height, LogicalUnit.Pixel);
                else if (e.KeyCode == Keys.Right)
                    newZone.Bounds = new LogicalRectangle(selectedZone.Bounds.Left + 1, selectedZone.Bounds.Top, selectedZone.Bounds.Width, selectedZone.Bounds.Height, LogicalUnit.Pixel);
                else
                    ignoreKeystroke = true;

                if (ignoreKeystroke == false)
                {
                    ocrPageProfessional.Zones.Insert(int.Parse(selectedZone.Id.ToString()), newZone);
                    selectedZone = ocrPageProfessional.Zones[int.Parse(selectedZone.Id.ToString())];

					// Update the position/size in the current zone line as well.
                    if (lineOcrPage != null)
                    {
                        for (int index = 0; index < lineOcrPage.Zones.Count; index++)
                        {
                            if (selectedZone.Name.StartsWith(lineOcrPage.Zones[index].Name))
                            {
                                double top = lineOcrPage.Zones[index].Bounds.Top;
                                if (Math.Abs(top - selectedZone.Bounds.Top) < 10) top = selectedZone.Bounds.Top;
                                lineOcrPage.Zones.RemoveAt(index);
                                newZone = selectedZone;
                                newZone.Bounds = new LogicalRectangle(selectedZone.Bounds.Left, top, selectedZone.Bounds.Width, selectedZone.Bounds.Height, LogicalUnit.Pixel);
                                lineOcrPage.Zones.Insert(index, newZone);
                                lineOcrPage.SaveZones(zoneLineFile);
                            }
                        }
                    }

                    viewer.Invalidate();
                }
            }

            if (e.KeyCode.ToString() == "Next")
            {
                toolStripButtonNextPage.PerformClick();
            }
            else if (e.KeyCode.ToString() == "PageUp")
            {
                toolStripButtonPreviousPage.PerformClick();
            }
        }

        private void viewer_MouseUp(object sender, MouseEventArgs e)
        {
			// Remember mouse click position for rubber band ending event.
            mouseUpY = e.Y;
            mouseUpX = e.X;
        }

        private void pictureBoxAdjust_Click(object sender, EventArgs e)
        {
			// Fade the help zone key lengend away using the timer, starting at 41 and going backwards.
            fadeCounter = 41;
        }

        private void contextMenuStripViewer_Opening(object sender, CancelEventArgs e)
        {
			// Apply option is available if selectedZone is not an actual zone.
            applyToolStripMenuItem.Enabled = selectedZone.Bounds.Right == selectedZone.Bounds.Left;
			// Delete zone option is available only if selectedZone is an actual zone.
            deleteToolStripMenuItem.Enabled = selectedZone.Bounds.Right > selectedZone.Bounds.Left;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ocrPageProfessional.Zones.Remove(selectedZone);
            viewer.Invalidate();
        }

        private void saveZoneLine(object sender, EventArgs e)
        {
			// Save the ozf file.
            if (newZoneLine == false)
                zoneLineFile = batchPath + "\\ApplyZones.ozf";

            if (File.Exists(zoneLineFile)) File.Delete(zoneLineFile);

			// Use the pages zones if lineOcrPage doesn't exist.
            if (lineOcrPage == null)
            {
                ocrPageProfessional.SaveZones(zoneLineFile);
            }
            else
            {
				// Add the zones on the current page to lineOcrPage, then save the ozf.
                int zoneCount = lineOcrPage.Zones.Count;
                lineOcrPage.Zones.Clear();

                for (int index = 0; index < zoneCount; index++)
                {
                    lineOcrPage.Zones.Add(ocrPageProfessional.Zones[index]);
                }
                lineOcrPage.SaveZones(zoneLineFile);
            }

			// Reload the zones.
            loadZoneLine();
        }

        private void toolStripTextBoxPageNumber_TextChanged(object sender, EventArgs e)
        {
			// Save page number as the last page viewed in batch config.
            if (toolStripTextBoxPageNumber.Text != "" && batchPath != "" && openingBatch == false)
            {
                string batchConfigFile = batchPath + "\\batch.config", batchTempFile = batchPath + "\\batch.tmp";
                string pageValue = toolStripTextBoxPageNumber.Text;
                string line;
                bool foundLastPage = false;

                if (File.Exists(batchConfigFile))
                {
                    StreamWriter sw = new StreamWriter(batchTempFile, false);

                    using (StreamReader sr = new StreamReader(batchConfigFile))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.StartsWith("LastPage"))
                            {
                                sw.WriteLine("LastPage=" + pageValue);
                                foundLastPage = true;
                            }
                            else
                                sw.WriteLine(line);
                        }
                    }

                    if (foundLastPage == false)
                        sw.WriteLine("LastPage=" + pageValue);

                    sw.Close();
                    File.Delete(batchConfigFile);
                    File.Move(batchTempFile, batchConfigFile);
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Automatically save when closing CamGenie.
            if (batchName != "")
                SavePageZones();

            if (modifiedImage == true)
                SaveImage();
        }

        private void newZoneLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// Prompt for the name of a new zone line which will be based on the current page.
            toolStripButtonClearZones.PerformClick();
            int pageIndex = toolStripTextBoxPageNumber.Text == "" ? 0 : int.Parse(toolStripTextBoxPageNumber.Text) - 1;
            InputBox inputBox = new InputBox("Zone Line Name", "ApplyZones");
            inputBox.ShowDialog(this);
            string newZoneLineFile = inputBox.InputValue;
            zoneLineFile = batchPath + "\\" + newZoneLineFile + ".ozf";
            toolStripStatusLabelApplyZone.Text = newZoneLineFile + ".ozf";
            RasterImage image = codecs.Load(images[pageIndex]);
            lineOcrPage = ocrEngineProfessional.CreatePage(image, OcrImageSharingMode.AutoDispose);
			// Use the InputBox provided file name when saving.
            newZoneLine = true;
        }

        private void clearOCRToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// Delete all records from the Field table that were created by the OCR step.
            if (MessageBox.Show(this, "Do you want to delete all OCR results for this batch? This will include results that have been confirmed during QC.", "Clear OCR", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                OleDbConnection oleDbConnection = new OleDbConnection(connectionString);
                OleDbCommand oleDbCommand;
                oleDbConnection.Open();
                oleDbCommand = new OleDbCommand("delete from Field where Batch = '" + batchName + "'", oleDbConnection);
                oleDbCommand.ExecuteNonQuery();
                oleDbConnection.Close();
                Messager.ShowInformation(this, "OCR has been cleared.");
            }
        }

        private void ocrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lineOcrPage == null || lineOcrPage.Zones.Count == 0)
            {
                Messager.ShowInformation(this, "Load zones before running OCR.");
                return;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            string zonePageFile, sql, srcFile, textValueProfessional, textValueAdvantage, fieldName, begdoc, zoneID;
            int pageCounter = 1, confidenceProfessional, confidenceAdvantage, fieldID, currentY;
            IOcrPage ocrPageOCRProfessional = null, ocrPageOCRAdvantage = null;
            IOcrPageCharacters ocrPageCharactersProfessional, ocrPageCharactersAdvantage;
            IOcrZoneCharacters ocrZoneCharacters;
            OleDbConnection oleDbConnection = new OleDbConnection(connectionString);
            OleDbCommand oleDbCommand;
            oleDbConnection.Open();
            string[] imageFiles = Directory.GetFiles(batchPath, "*.tif");

			// Clear out old OCR and any uploaded Final QC results.
            if (menuItem.Equals(ocrBatchToolStripMenuItem))
            {
                oleDbCommand = new OleDbCommand("delete from " + batchTableName + " where Batch = '" + batchName + "'", oleDbConnection);
                oleDbCommand.ExecuteNonQuery();
                oleDbCommand = new OleDbCommand("delete from Field where Batch = '" + batchName + "'", oleDbConnection);
                oleDbCommand.ExecuteNonQuery();
            }
            else
			// Clear out only for the current page.
            {
                oleDbCommand = new OleDbCommand("delete from Field where Batch = '" + batchName + "' and Page = " + toolStripTextBoxPageNumber.Text, oleDbConnection);
                oleDbCommand.ExecuteNonQuery();
                int pageIndex = int.Parse(toolStripTextBoxPageNumber.Text) - 1;
                string imageFile = imageFiles[pageIndex];
                imageFiles = new string[1];
                imageFiles[0] = imageFile;
                pageCounter = currentPageNumber;
            }

			// OCR by TIF file in the batch.
            foreach (string imageFile in imageFiles)
            {
				// Zone files exist per page with all applied zones named after the begdoc# for the page.
                begdoc = imageFile.Substring(0, imageFile.LastIndexOf("."));
                zonePageFile = begdoc + ".ozf";
                begdoc = begdoc.Substring(begdoc.LastIndexOf("\\") + 1);
                
                if (imageFiles.Length == 1)
                    toolStripStatusLabel.Text = "OCR " + imageFile + ", page " + pageCounter.ToString();
                else
                    toolStripStatusLabel.Text = "OCR " + imageFile + ", page " + pageCounter.ToString() + " of " + imageFiles.Length.ToString();

                Application.DoEvents();

                if (File.Exists(zonePageFile))
                {
                    RasterImage image = codecs.Load(imageFile);

                    if (image.XResolution < 150)
                        image.XResolution = 150;

                    if (image.YResolution < 150)
                        image.YResolution = 150;

					// Load zones into an OcrPage just used for OCR purposes.
                    ocrPageOCRProfessional = ocrEngineProfessional.CreatePage(image, OcrImageSharingMode.None);
                    ocrPageOCRProfessional.LoadZones(zonePageFile);
                    double topY = -1;
                    zonesPerLine = 0; 

                	// Count the number of zones in a line for this page.
                    foreach (OcrZone zone in ocrPageOCRProfessional.Zones)
                    {
                        if (topY == -1)
                            topY = zone.Bounds.Top;

                        if (zone.Bounds.Top > topY + 10)
                            break;
                        else
                            zonesPerLine++;
                    }

					// Perform OCR.
                    ocrPageOCRProfessional.Recognize(null);
                    ocrPageOCRAdvantage = ocrEngineAdvantage.CreatePage(image, OcrImageSharingMode.None);
                    ocrPageOCRAdvantage.LoadZones(zonePageFile);
                    ocrPageOCRAdvantage.Recognize(null);
                    ocrPageCharactersProfessional = ocrPageOCRProfessional.GetRecognizedCharacters();
                    ocrPageCharactersAdvantage = ocrPageOCRAdvantage.GetRecognizedCharacters();

					// Determine total number of lines to save to batchTableName as placeholders, to be filled after Main QC.
                    int lineTotal = ocrPageOCRProfessional.Zones.Count / zonesPerLine, line, zoneIndex;
                    string insertValues, fieldInsertValues;

                    if (menuItem.Equals(ocrBatchToolStripMenuItem))
                    {
                        for (int lineCounter = 0; lineCounter < lineTotal; lineCounter++)
                        {
                            line = lineCounter + 1;
                            insertValues = "'" + batchName + "', " + pageCounter.ToString() + ", " + line.ToString() + ", ";

                            //if (cityState.Length > 3)
                            //{
                            //    if (cityState.Substring(cityState.Length - 3, 1) == " ")
                            //        insertValues += "'" + cityState.Substring(0, cityState.Length - 4).Replace(Environment.NewLine, "").Replace("'", "''") + "', '" + cityState.Substring(cityState.Length - 2, 2) + "', ";
                            //    else
                            //        insertValues += "'" + cityState.Replace(Environment.NewLine, "").Replace("'", "''") + "', '<None>', ";
                            //}
                            //else
                            //{
                            //    insertValues += "'" + cityState.Replace(Environment.NewLine, "").Replace("'", "''") + "', '<None>', ";
                            //}

                            srcFile = "";

							// Initialize source file value.
                            if (imageFile.Split('\\').Length == 5)
                                srcFile = imageFile.Split('\\')[2] + "\\" + imageFile.Split('\\')[3] + "\\" + imageFile.Split('\\')[4];
                            else
                            {
                                try
                                {
                                    srcFile = imageFile.Split('\\')[2] + "\\" + imageFile.Split('\\')[3];
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error parsing file path " + imageFile + ", error: " + ex.Message);
                                }
                            }

							// Add row to batchTableName with key fields, Srcfile and Begdoc#.
                            insertValues += "'" + srcFile + "', '" + begdoc + "'";
                            sql = "insert into " + batchTableName + " (Batch, Page, Line, Srcfile, Begdoc#) values (" + insertValues + ")";
                            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                            oleDbCommand.ExecuteNonQuery();
                        }
                    }

                    zoneIndex = 0;

					// Insert rows into Field table with OCR results for the current image file.
                    foreach (OcrZone zone in ocrPageOCRProfessional.Zones)
                    {
                        fieldName = zone.Name;
                        fieldID = GetFieldID(fieldName);
                        ocrZoneCharacters = ocrPageCharactersProfessional.FindZoneCharacters(zone.Id);
                        confidenceProfessional = 0;
                        textValueProfessional = "";
                        line = zoneIndex / zonesPerLine + 1;

						// Build OCR text value from characters collection and confidence score from Professional engine.
                        if (ocrZoneCharacters != null)
                        {
                            foreach (OcrCharacter ocrCharacter in ocrZoneCharacters)
                            {
                                if (confidenceProfessional == 0 || confidenceProfessional > ocrCharacter.Confidence)
                                    confidenceProfessional = ocrCharacter.Confidence;

                                textValueProfessional += ocrCharacter.Code.ToString();
                            }
                        }

                        ocrZoneCharacters = ocrPageCharactersAdvantage.FindZoneCharacters(zoneIndex);
                        confidenceAdvantage = 0;
                        textValueAdvantage = "";

						// Build OCR text value from characters collection and confidence score from Advantage engine.
                        if (ocrZoneCharacters != null)
                        {
                            foreach (OcrCharacter ocrCharacter in ocrZoneCharacters)
                            {
                                if (confidenceAdvantage == 0 || confidenceAdvantage > ocrCharacter.Confidence)
                                    confidenceAdvantage = ocrCharacter.Confidence;

                                textValueAdvantage += ocrCharacter.Code.ToString();
                            }
                        }

                        fieldInsertValues = "'" + batchName + "', " + pageCounter.ToString() + ", " + line.ToString() + ", " + fieldID.ToString() + ", '" +
                                            zone.Id.ToString() + "', '" + textValueProfessional.Replace("'", "''") + "', " + confidenceProfessional.ToString() + ", '" +
                                            textValueAdvantage.Replace("'", "''") + "', " + confidenceAdvantage.ToString() + ", 0, " + zone.Bounds.Top.ToString();

                        sql = "insert into Field values (" + fieldInsertValues + ")";
                        oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                        oleDbCommand.ExecuteNonQuery();

                        zoneIndex += 1;
                    }

                    int holdY = 0, lineCtr = 1;
                    sql = "select ZoneID, Y from Field where Batch = '" + batchName + "' and Page = " + pageCounter.ToString() + " order by Y";
                    oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                    OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
                    DataTable dataTable = new DataTable();
                    oleDbDataAdapter.Fill(dataTable);

					// Update Line field by going through zones and detecting when a zone is significantly lower to justify a change in line.
                    foreach (DataRow row in dataTable.Rows)
                    {
                        currentY = int.Parse(row[1].ToString());
                        zoneID = row[0].ToString();

                        if (holdY == 0)
                            holdY = currentY;

                        if (!(currentY >= holdY - 20 && currentY <= holdY + 20))
                        {
                            lineCtr++;
                            holdY = currentY;
                        }

                        sql = "update Field set Line = " + lineCtr.ToString() + " where Batch = '" + batchName + "' and Page = " + pageCounter.ToString() + " and ZoneID = " + zoneID;
                        oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                        oleDbCommand.ExecuteNonQuery();
                    }
                }

                pageCounter += 1;
                ocrQCToolStripMenuItem.Enabled = true;

                if (imageFiles.Length > 1)
                    toolStripStatusLabel.Text = "OCR " + imageFile + ", page " + pageCounter.ToString() + " of " + imageFiles.Length.ToString();

                Application.DoEvents();
            }

			// Cleanup.
            toolStripStatusLabel.Text = "";
            oleDbConnection.Close();

            if (ocrPageOCRProfessional != null)
                ocrPageOCRProfessional.Dispose();

            Messager.ShowInformation(this, "OCR is complete.");
            currentPageNumber = int.Parse(toolStripTextBoxPageNumber.Text);
            toolStripStatusLabel.Text = imageFilePaths[currentPageNumber - 1];
            toolStripButtonApplyZones.Enabled = false;
            toolStripButtonQuickApplyZones.Enabled = false;
        }

        private void clearBatchZonesToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// Delete all zone line files for the current batch.
            if (MessageBox.Show(this, "All zones for the entire batch will be deleted. Continue?", "Clear Batch Zones", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                string[] files = Directory.GetFiles(batchPath, "*.ozf");

                foreach (string file in files)
                {
                    if (!file.Contains("ApplyZones"))
                        File.Delete(file);
                }

                ocrPageProfessional.Zones.Clear();
                viewer.Invalidate();
                Messager.ShowInformation(this, "Batch zones have been cleared.");
            }
        }

        private void toolStripTextBoxLinesPerPage_TextChanged(object sender, EventArgs e)
        {
            toolStripButtonQuickApplyZones.Enabled = toolStripTextBoxLinesPerPage.Text.Trim() != "";

			// Add or update LinesPerPage to batch config file.
            if (toolStripTextBoxLinesPerPage.Text != "" && batchPath != "" && openingBatch == false)
            {
                string batchConfigFile = batchPath + "\\batch.config", batchTempFile = batchPath + "\\batch.tmp";
                string linesPerPageValue = toolStripTextBoxLinesPerPage.Text;
                string line;
                bool foundLinesPerPage = false;

                if (File.Exists(batchConfigFile))
                {
                    StreamWriter sw = new StreamWriter(batchTempFile, false);

                    using (StreamReader sr = new StreamReader(batchConfigFile))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.StartsWith("LinesPerPage"))
                            {
                                sw.WriteLine("LinesPerPage=" + linesPerPageValue);
                                foundLinesPerPage = true;
                            }
                            else
                                sw.WriteLine(line);
                        }
                    }

                    if (foundLinesPerPage == false)
                        sw.WriteLine("LinesPerPage=" + linesPerPageValue);

                    sw.Close();
                    File.Delete(batchConfigFile);
                    File.Move(batchTempFile, batchConfigFile);
                }
            }
        }

        private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// Display dialog for uploading Main QC results to Final QC (batchTableName).
            Upload upload = new Upload();
            upload.BatchName = batchName;
            upload.BatchType = batchType;
            upload.BatchPath = batchPath;
            upload.PageTotal = images.Length;
            upload.mainForm = this;
            upload.Show(this);
        }

        private void batchPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Display batch properties dialog.
            BatchProperties batchProperties = new BatchProperties();
            batchProperties.BatchName = batchName;
            batchProperties.BatchPath = batchPath;

            if (batchProperties.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                batchType = batchProperties.BatchType;
        }

        private void ocrQCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lineOcrPage == null || lineOcrPage.Zones.Count == 0)
            {
                Messager.ShowInformation(this, "Load zones before running QC.");
                return;
            }

            string batchConfigFile = batchPath + "\\batch.config", fieldValue;
            bool foundHeaderFields = false;

			// Make sure header fields were entered before starting Main QC.
            if (batchType == "PhoneBill")
            {
                if (File.Exists(batchConfigFile))
                {
                    string line;

                    using (StreamReader sr = new StreamReader(batchConfigFile))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            fieldValue = line.Split('=')[1];

                            if (line.StartsWith("Year") && fieldValue != "")
                            {
                                foundHeaderFields = true;
                                break;
                            }
                        }
                    }
                }

                if (foundHeaderFields == false)
                {
                    Messager.ShowInformation(this, "Enter the header values in File->Batch Properties before running QC.");
                    return;
                }
            }

			// Confirm that OCR was run already.
            OleDbConnection oleDbConnection = new OleDbConnection(connectionString);
            oleDbConnection.Open();
            string sql = "select ValueAdvantage from Field where Batch = '" + batchName + "'";
            OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
            DataTable dataTable = new DataTable();
            oleDbDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count == 0)
            {
                Messager.ShowInformation(this, "Run OCR before running QC.");
                return;
            }

			// Confirm that Main QC was not already completed.
            sql = "select ValueAdvantage from Field where Batch = '" + batchName + "' and Confirmed = 0 and ValueAdvantage != ''";
            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
            dataTable = new DataTable();
            oleDbDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count == 0)
            {
                Messager.ShowInformation(this, "QC has already been completed. All values have been confirmed.");
                return;
            }

			// Initialize batch header values and show Main QC.
            oleDbConnection.Close();
            this.Cursor = Cursors.AppStarting;
            OCRQC qc = new OCRQC();
            qc.BatchPath = batchPath;
            qc.BatchName = batchName;
            qc.BatchType = batchType;
            qc.CallDateYear = callDateYear;
            qc.OcrEngine = ocrEngineProfessional;
            qc.LineOcrPage = lineOcrPage;
            qc.mainForm = this;
            this.Left = 0;
            qc.Left = this.Width - 10;
            qc.Top = this.Top;
            qc.Show(this);
            this.Cursor = Cursors.Default;
        }

        private void finalQCToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// Display Final QC window with necessary batch parameters.
            this.Cursor = Cursors.AppStarting;
            FinalQC qc = new FinalQC();
            qc.BatchName = batchName;
            qc.BatchType = batchType;
            qc.mainForm = this;
            this.Left = 0;
            qc.Left = this.Width - 10;
            qc.Top = this.Top;
            qc.Show(this);
            this.Cursor = Cursors.Default;
        }

        private void toolStripButtonIncreaseSpacing_Click(object sender, EventArgs e)
        {
			// Increase spacing and re-apply.
            double spacing = double.Parse(toolStripComboBoxSpacing.Text);
            spacing += 0.1;
            toolStripComboBoxSpacing.Text = spacing.ToString();
            toolStripButtonQuickApplyZones.PerformClick();
        }

        private void toolStripButtonDecreaseSpacing_Click(object sender, EventArgs e)
        {
			// Decrement spacing and re-apply.
			double spacing = double.Parse(toolStripComboBoxSpacing.Text);

            if (spacing > 0)
            {
                spacing -= 0.1;
                toolStripComboBoxSpacing.Text = spacing.ToString();
                toolStripButtonQuickApplyZones.PerformClick();
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// Display export dialog.
            saveFileDialogExport.ShowDialog(this);
        }

        private void saveFileDialogExport_FileOk(object sender, CancelEventArgs e)
        {
			// Export batch data to csv file
            if (saveFileDialogExport.FileName != "")
            {
                string exportFile = saveFileDialogExport.FileName;
                exportFile = exportFile.Replace(".csv", "-1.csv");
                StreamWriter sw = new StreamWriter(exportFile);

                string line, delim;
                int lineCounter = 0, lineTotal = 0, fileCounter = 1;
                OleDbConnection oleDbConnection = new OleDbConnection(connectionString);
                oleDbConnection.Open();
                string projectName = batchName.Split(' ')[0], sql;

                sql = "select count(*) from " + batchTableName + " where Batch like '" + projectName + "%'";
                OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
                DataTable dataTable = new DataTable();
                oleDbDataAdapter.Fill(dataTable);
                lineTotal = int.Parse(dataTable.Rows[0][0].ToString());

                if (batchType == "PhoneBill")
                {
                    sql = "select CONVERT(varchar(12), Calldate, 101) Calldate, Calltime, Callfrom, Callto, Calltype, City, " +
                          "[State], Duration, Acctno, Begdoc#, Company, Custodian, VolInfo, ATR_Load_Info, TimeZone from Phone where Batch " +
                          "like '" + projectName + "%' and Calldate is not null order by Calldate, Calltime";
                    sw.WriteLine("Calldate,Calltime,Callfrom,Callto,Calltype,City,State,Duration,Acctno,Begdoc#,Company,Custodian,VolInfo,ATRLoadInfo,TimeZone");
                }
                else if (batchType == "BankStatement")
                {
                    sql = "select AccountNumber, CardNumber, RoutingNumber, AccountHolder1, AccountHolder2, TransactionType, CONVERT(varchar(12), TransactionDate, 101) TransactionDate, " +
                          "CheckNumber, Amount, DepositAmount, Description, Begdoc#, Company, Custodian, VolInfo, ATR_Load_Info from BankStatement where Batch " +
                          "like '" + projectName + "%' and TransactionDate is not null order by AccountNumber, CardNumber, RoutingNumber, AccountHolder1, AccountHolder2, TransactionType, TransactionDate";
                    sw.WriteLine("AccountNumber,CardNumber,RoutingNumber,AccountHolder1,AccountHolder2,TransactionType,TransactionDate,CheckNumber,Amount,DepositAmount,Description,Begdoc#,Company,Custodian,VolInfo,ATRLoadInfo");
                }

                oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                oleDbCommand.CommandTimeout = 6400;
                OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader(CommandBehavior.KeyInfo);

                while (oleDbDataReader.Read())
                {
                    line = "";
                    delim = "";

                    for (int index = 0; index < oleDbDataReader.FieldCount - 3; index++)
                    {
                        line += delim + oleDbDataReader[index].ToString().Replace(",", " ");
                        delim = ",";
                    }

                    sw.WriteLine(line);
                    lineCounter++;

                    if (lineCounter % 100 == 0)
                    {
                        toolStripStatusLabel.Text = "Exported " + lineCounter.ToString("#,0") + " of " + lineTotal.ToString("#,0");
                        Application.DoEvents();
                    }

                    if (lineCounter % 1000000 == 0)
                    {
                        sw.Close();
                        int nextFileNumber = fileCounter + 1;
                        exportFile = exportFile.Replace(fileCounter.ToString() + ".csv", nextFileNumber.ToString() + ".csv");
                        fileCounter++;
                        sw = new StreamWriter(exportFile);
                    }
                }

                sw.Close();
                toolStripStatusLabel.Text = "Exported " + lineTotal.ToString("#,0") + " of " + lineTotal.ToString("#,0");
                MessageBox.Show("Export is complete.");
                toolStripStatusLabel.Text = "";
            }
        }

        private void deleteLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// Delete all zones in the line that contains the selectedZone.
            double selectedTop = selectedZone.Bounds.Top;
            bool foundZone = false;

            while (true)
            {
                for (int index = 0; index < ocrPageProfessional.Zones.Count; index++)
                {
                    if (ocrPageProfessional.Zones[index].Bounds.Top > selectedTop - 20 && ocrPageProfessional.Zones[index].Bounds.Top < selectedTop + 20)
                    {
                        ocrPageProfessional.Zones.RemoveAt(index);
                        foundZone = true;
                        break;
                    }
                }

                if (foundZone == false)
                    break;
                else
                    foundZone = false;
            }

            viewer.Invalidate();

        }

        private void deleteToBottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// Delete all lines starting with the line of the selected zone down.
            double selectedTop = selectedZone.Bounds.Top, selectedLeft = selectedZone.Bounds.Left;

            if (selectedTop == 0)
            {
                Messager.ShowInformation(this, "Right click a zone before deleting to bottom.");
                return;
            }

            bool foundZone = false;

            while (true)
            {
                for (int index = 0; index < ocrPageProfessional.Zones.Count; index++)
                {
                    if (ocrPageProfessional.Zones[index].Bounds.Top > selectedTop - 5 && ocrPageProfessional.Zones[index].Bounds.Left > selectedLeft - 5)
                    {
                        ocrPageProfessional.Zones.RemoveAt(index);
                        foundZone = true;
                        break;
                    }
                }

                if (foundZone == false)
                    break;
                else
                    foundZone = false;
            }

            viewer.Invalidate();

        }

        private void importBatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportBatch importBatch = new ImportBatch();
            importBatch.Show(this);
        }

        private void deskewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
				// Perform LEADTOOLS deskew and save the TIF file.
                DeskewCommand _Deskew = new DeskewCommand();
                // Update the DeskewCommandFlags, these flags will indicate whether to deskew the image, which background color to use, whether to deskew the image if the skew angle is very small, which type of interpolation to use, whether the image contains mostly text, and whether to use normal or fast rotation.
                _Deskew.Flags = (DeskewCommandFlags)0;
                _Deskew.Flags |= DeskewCommandFlags.DoNotFillExposedArea;
                _Deskew.Flags |= DeskewCommandFlags.NoThreshold;
                _Deskew.Flags |= DeskewCommandFlags.RotateBicubic;
                _Deskew.Flags |= DeskewCommandFlags.DocumentImage;
                _Deskew.Flags |= DeskewCommandFlags.UseNormalRotate;
                // Run the DeskewCommand on the loaded image.
                _Deskew.Run(viewer.Image);
                SaveImage();
            }
            catch (RasterException ex)
            {
                Messager.ShowError(this, ex);
            }
        }

        private void rotateRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// Perform LEADTOOLS rotate right and save the TIF file.
            RotateCommand _Rotate = new RotateCommand(90 * 100, RotateCommandFlags.Resize, new RasterColor(0, 0, 0));
            _Rotate.Run(viewer.Image);
            SaveImage();
        }

        private void SaveImage()
        {
            // Copy a backup for the Undo menu option.
            File.Copy(imageFilePaths[currentPageNumber - 1], imageFilePaths[currentPageNumber - 1] + ".bak");
            codecs.Save(viewer.Image, imageFilePaths[currentPageNumber - 1], Leadtools.RasterImageFormat.Tif, 0);
        }

        private void rotateLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// Perform LEADTOOLS rotate left and save the TIF file.
            RotateCommand _Rotate = new RotateCommand(-90 * 100, RotateCommandFlags.Resize, new RasterColor(0, 0, 0));
            _Rotate.Run(viewer.Image);
            SaveImage();
        }

        private void quickApplyToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// Most of apply code is used when performing a Quick Apply.
            contextClickedY = clickedY;
            contextClickedX = clickedX;
            toolStripButtonApplyZones_Click(toolStripButtonQuickApplyZones, null);
        }

        private void updateYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Used to fix line numbers according to current location of zones.
            string y, sql, zoneID;
            int holdY = 0, currentY = 0, lineCounter = 1;
            OleDbCommand oleDbCommand;
            OleDbConnection oleDbConnection = new OleDbConnection(connectionString);
            oleDbConnection.Open();
            
            foreach (OcrZone zone in ocrPageProfessional.Zones)
            {
                y = zone.Bounds.Top.ToString();
                sql = "update Field set Y = " + y + " where Batch = '" + batchName + "' and Page = " + currentPageNumber.ToString() + " and ZoneID = " + zone.Id.ToString();                
                oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                oleDbCommand.ExecuteNonQuery();
            }

            sql = "select ZoneID, Y from Field where Batch = '" + batchName + "' and Page = " + currentPageNumber.ToString() + " order by Y";
            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
            DataTable dataTable = new DataTable();
            oleDbDataAdapter.Fill(dataTable);

            foreach (DataRow row in dataTable.Rows)
            {
                currentY = int.Parse(row[1].ToString());
                zoneID = row[0].ToString();

                if (holdY == 0)
                    holdY = currentY;

                if (!(currentY >= holdY - 20 && currentY <= holdY + 20))
                {
                    lineCounter++;
                    holdY = currentY;
                }

                sql = "update Field set Line = " + lineCounter.ToString() + " where Batch = '" + batchName + "' and Page = " + currentPageNumber.ToString() + " and ZoneID = " + zoneID;
                oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                oleDbCommand.ExecuteNonQuery();
            }

            oleDbConnection.Close();
            Messager.ShowInformation(this, "Done.");
        }

        private void lineRemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// Display dialog with settings for removing horizontal and vertical lines in an image.
            lineRemove = new LineRemove();

            if (lineRemove.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
				// Perform the line removal.
                toolStripButtonLineRemoval_Click(null, null);
                toolStripButtonLineRemoval.Enabled = true;
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modifiedImage = false;

            // Restore the image backup and reload.
            if (File.Exists(imageFilePaths[currentPageNumber - 1] + ".bak"))
            {
                File.Delete(imageFilePaths[currentPageNumber - 1]);
                File.Move(imageFilePaths[currentPageNumber - 1] + ".bak", imageFilePaths[currentPageNumber - 1]);
            }

            LoadImage();
        }

        private void toolStripButtonLineRemoval_Click(object sender, EventArgs e)
        {
			// Perform LEADTOOLS line removal and save the TIF file.
            LineRemoveCommand _LineRemove = new LineRemoveCommand();

            _LineRemove.Flags = LineRemoveCommandFlags.UseGap;

            if (lineRemove.RemoveHorizontalLines == true)
                _LineRemove.Type = LineRemoveCommandType.Horizontal;
            else
                _LineRemove.Type = LineRemoveCommandType.Vertical;

            _LineRemove.MinimumLineLength = lineRemove.MinimumLength;
            _LineRemove.MaximumLineWidth = lineRemove.MaximumWidth;
            _LineRemove.GapLength = lineRemove.MaximumGap;

            try { _LineRemove.Run(viewer.Image); }
            catch (Exception ex) { Messager.ShowError(this, ex); }

            if (lineRemove.RemoveHorizontalLines == true && lineRemove.RemoveVerticalLines == true)
                _LineRemove.Type = LineRemoveCommandType.Vertical;

            try { _LineRemove.Run(viewer.Image); }
            catch (Exception ex) { Messager.ShowError(this, ex); }

            modifiedImage = true;
            SaveImage();
        }

        private void viewer_MouseMove(object sender, MouseEventArgs e)
        {
			// Display mouse coordinates on status bar.
            toolStripStatusLabelCoordinates.Text = "X:" + e.Location.X.ToString() + ", Y:" + e.Location.Y.ToString();
            Application.DoEvents();
        }

        private void quickApplyOnceToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// Quick Apply for only one line. Do not continue according to lines per page.
            contextClickedY = clickedY;
            contextClickedX = clickedX;
            toolStripButtonApplyZones_Click(quickApplyOnceToolStripMenuItem, null);
        }

        private void viewer_SizeChanged(object sender, EventArgs e)
        {
			// Keep the viewer from extending behind the status bar.
            viewer.Height = this.Height - 120;
        }

        private void dotRemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
				// Perform LEADTOOLS dot (noise) removal and save the TIF file.
                DotRemoveCommand _DotRemove = null;
                DotRemove dlg = new DotRemove();
                // Open the DotRemoveCommand Dialog
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _DotRemove = new DotRemoveCommand();
                    // Update the DotRemoveCommand.MaximumDotHeight to set the maximum height of a dot to be removed.
                    _DotRemove.MaximumDotHeight = dlg.MaximumDotHeight;
                    // Update the DotRemoveCommand.MaximumDotWidth to set the maximum width of a dot to be removed.
                    _DotRemove.MaximumDotWidth = dlg.MaximumDotWidth;
                    // Update the DotRemoveCommand.MinimumDotHeight to set the minimum height of a dot to be removed.
                    _DotRemove.MinimumDotHeight = dlg.MinimumDotHeight;
                    // Update the DotRemoveCommand.MinimumDotWidth to set the minimum width of a dot to be removed.
                    _DotRemove.MinimumDotWidth = dlg.MinimumDotWidth;
                    // Run the command on loaded image.
                    _DotRemove.Run(viewer.Image);
                    SaveImage();
                }
            }
            catch (RasterException ex)
            {
                Messager.ShowError(this, ex);
            }
        }

        private void toolStripComboBoxSpacing_TextUpdate(object sender, EventArgs e)
        {
            // Respond as if a spacing value was selected from the drop down list when a spacing is entered manually.
            toolStripComboBoxSpacing_SelectedIndexChanged(toolStripComboBoxSpacing, null);
        }

        private void miscToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// General DGWeb/CG utility code. 
            OleDbConnection oleDbConnection = new OleDbConnection(connectionString);
            oleDbConnection.Open();
            string sql = "select * from CustodianPhoneListFlatNew";
            OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
            DataTable dataTable = new DataTable();
            oleDbDataAdapter.Fill(dataTable);
            sql = "update CustodianPhoneListFlatNew set ";

            foreach (DataRow row in dataTable.Rows)
            {
                string custodian = row["Custodian"].ToString(), lastName = "", firstName = "";
                string id = row["ID"].ToString();

                if (custodian.Split(',').Length > 1)
                {
                    lastName = custodian.Split(',')[0].Trim().Replace("'", "''");
                    firstName = custodian.Split(',')[1].Trim().Replace("'", "''");
                }
                else if (custodian.Split(',').Length == 1)
                {
                    lastName = custodian.Replace("'", "''").Trim();
                }
                oleDbCommand = new OleDbCommand(sql + "LastName = '" + lastName + "', FirstName = '" + firstName + "' where ID = " + id, oleDbConnection);
                oleDbCommand.ExecuteNonQuery();
            }

            oleDbConnection.Close();
            MessageBox.Show("Done.");

        }
    }
}
