using Circuit_Simulator.COMP;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Circuit_Simulator.UI.UI_Configs;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_EditComp_Window : UI_Window
    {
        public CompData rootcomp;
        public UI_ValueInput Box_Name, Box_Category, Box_SimCode_FuncName, Box_AfterSimCode_FuncName, Box_InternalState_Length, Box_ClickType, ComponentValueInputCount;
        public UI_Scrollable<UI_Element> Features;
        List<UI_ValueInput> inputlist = new List<UI_ValueInput>();
        List<UI_String> labellist = new List<UI_String>();
        UI_StringButton Code_Sim_Button, Code_AfterSim_Button;
        UI_StringButton[] rotbuttons;
        UI_TexButton[] paintbuttons;
        UI_GridPaint gridpaint;
        UI_String InputCount_Label;
        List<string> parameterlabels;

        Color[] paintbuttoncols = new Color[] { new Color(0.5f, 0.5f, 0.5f, 1.0f), new Color(0.25f, 0.25f, 0.25f, 1.0f), new Color(0.8f, 0.8f, 0.8f, 1.0f), new Color(0.15f, 0.15f, 0.15f, 1.0f), new Color(1.0f, 1.0f, 0.0f, 1.0f) };

        //Code Boxes
        public static UI_TextBox CodeBox_Sim, CodeBox_AfterSim;

        public UI_EditComp_Window(Pos pos, Point size, string title, Point minsize, Generic_Conf conf, bool IsResizeable) : base(pos, size, title, minsize, conf, IsResizeable )
        {

            UI_String spooky = new UI_String(new Pos(0), Point.Zero, conf, "");
            UI_String Box_Name_Label = new UI_String(new Pos(bezelsize, bezelsize + headheight), Point.Zero, UI_Handler.genbutconf, "Name: ");
            UI_String Box_Category_Label = new UI_String(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, Box_Name_Label), Point.Zero, UI_Handler.genbutconf, "Category: ");
          
            Box_Name = new UI_ValueInput(new Pos(0, ORIGIN.TR, ORIGIN.DEFAULT, Box_Name_Label), new Point(size.X / 2, (int)(UI_Handler.genbutconf.font.MeasureString("Test").Y)), UI_Handler.genbutconf, 3);
            Box_Name.ValueChanged += BoxName_ValueChange;
            Box_Category = new UI_ValueInput(new Pos(0, ORIGIN.TR, ORIGIN.DEFAULT, Box_Category_Label), new Point(size.X / 2, (int)(UI_Handler.genbutconf.font.MeasureString("Test").Y)), UI_Handler.genbutconf, 3);
            Box_Category.ValueChanged += BoxCategory_ValueChange;

            Features = new UI_Scrollable<UI_Element>(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, Box_Category_Label), Point.Zero);
            UI_String SimCode_FuncName_Label = new UI_String(new Pos(5, 5), Point.Zero, UI_Handler.genbutconf, "Sim Code Func. Name: ");
            Box_SimCode_FuncName = new UI_ValueInput(new Pos(0, ORIGIN.TR, ORIGIN.DEFAULT, SimCode_FuncName_Label), new Point(size.X / 2, SimCode_FuncName_Label.size.Y), UI_Handler.genbutconf, 3);
            UI_String AfterSimCode_FuncName_Label = new UI_String(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, SimCode_FuncName_Label), Point.Zero, UI_Handler.genbutconf, "After-Sim Code Func. Name: ");
            Box_AfterSimCode_FuncName = new UI_ValueInput(new Pos(0, ORIGIN.TR, ORIGIN.DEFAULT, AfterSimCode_FuncName_Label), new Point(size.X / 2, AfterSimCode_FuncName_Label.size.Y), UI_Handler.genbutconf, 3);
            UI_String InternalState_Length_Label = new UI_String(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, AfterSimCode_FuncName_Label), Point.Zero, UI_Handler.genbutconf, "Internal State Length: ");
            Box_InternalState_Length = new UI_ValueInput(new Pos(0, ORIGIN.TR, ORIGIN.DEFAULT, InternalState_Length_Label), new Point(size.X / 2, AfterSimCode_FuncName_Label.size.Y), UI_Handler.genbutconf, 1);
            UI_String ClickType_Label = new UI_String(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, InternalState_Length_Label), Point.Zero, UI_Handler.genbutconf, "Click Type: ");
            Box_ClickType = new UI_ValueInput(new Pos(0, ORIGIN.TR, ORIGIN.DEFAULT, ClickType_Label), new Point(size.X / 2, AfterSimCode_FuncName_Label.size.Y), UI_Handler.genbutconf, 1);


            Code_Sim_Button = new UI_StringButton(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, ClickType_Label), new Point((int)(UI_Handler.buttonwidth * 1.8), UI_Handler.buttonheight), "Edit Sim Code", true, UI_Handler.genbutconf);
            Code_AfterSim_Button = new UI_StringButton(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, Code_Sim_Button), new Point((int)(UI_Handler.buttonwidth * 2.4), UI_Handler.buttonheight), "Edit After-Sim Code", true, UI_Handler.genbutconf);

            rotbuttons = new UI_StringButton[4];
            for(int i = 0; i < 4; ++i)
            {
                if(i == 0)
                    rotbuttons[i] = new UI_StringButton(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, Code_AfterSim_Button), new Point(25, 25), i.ToString(), true, UI_Handler.behave1conf);
                else
                    rotbuttons[i] = new UI_StringButton(new Pos(5, 0, ORIGIN.TR, ORIGIN.DEFAULT, rotbuttons[i - 1]), new Point(25, 25), i.ToString(), true, UI_Handler.behave1conf);
                rotbuttons[i].GotToggledLeft += RotButtonPressed;
            }
            rotbuttons[0].IsActivated = true;
            gridpaint = new UI_GridPaint(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, rotbuttons[0]), new Point(450), 300, new Point(150), new Point(-2, 6), UI_Handler.gridpaintbuttonconf);
            gridpaint.UpdateFunctions.Add(delegate ()
            {
                if (new Rectangle(gridpaint.absolutpos, gridpaint.size).Contains(Game1.mo_states.New.Position))
                    Features.DenyScroll = true;
            });
            gridpaint.PixelChanged += PixelChanged;
            paintbuttons = new UI_TexButton[5];
            for (int i = 0; i < 5; ++i)
            {
                Generic_Conf curconf = new Generic_Conf(UI_Handler.gridpaintbuttonconf);
                curconf.tex_color = paintbuttoncols[i];
                if (i == 0)
                    paintbuttons[i] = new UI_TexButton(new Pos(5, 0, ORIGIN.TR, ORIGIN.DEFAULT, gridpaint), new Point(25, 25), new Point(364, 0), UI_Handler.Button_tex, curconf);
                else
                    paintbuttons[i] = new UI_TexButton(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, paintbuttons[i - 1]), new Point(25, 25), new Point(364, 0), UI_Handler.Button_tex, curconf);
                paintbuttons[i].GotToggledLeft += PaintButtonPressed;
            }
            paintbuttons[0].IsActivated = true;
            gridpaint.curplacetype = 1;

            InputCount_Label = new UI_String(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, gridpaint), Point.Zero, UI_Handler.genbutconf, "ParameterCount: ");
            ComponentValueInputCount = new UI_ValueInput(new Pos(0, ORIGIN.TR, ORIGIN.DEFAULT, InputCount_Label), new Point(size.X / 8, SimCode_FuncName_Label.size.Y), UI_Handler.genbutconf, 1, 2);

            parameterlabels = new List<string>();



            Features.Add_UI_Elements(spooky, SimCode_FuncName_Label, Box_SimCode_FuncName, AfterSimCode_FuncName_Label, Box_AfterSimCode_FuncName, InternalState_Length_Label, Box_InternalState_Length, ClickType_Label, Box_ClickType, Code_Sim_Button, Code_AfterSim_Button);
            Features.Add_UI_Elements(rotbuttons);
            Features.Add_UI_Elements(gridpaint);
            Features.Add_UI_Elements(paintbuttons);
            Features.Add_UI_Elements(InputCount_Label);
            Features.Add_UI_Elements(ComponentValueInputCount);
            Add_UI_Elements(Box_Name_Label, Box_Name, Box_Category_Label, Box_Category, Features);
            
            Code_Sim_Button.GotActivatedLeft += Code_Sim_Button_Pressed;
            Code_AfterSim_Button.GotActivatedLeft += Code_AfterSim_Button_Pressed;
            Box_SimCode_FuncName.ValueChanged += Box_SimCode_FuncName_ValueChange;
            Box_AfterSimCode_FuncName.ValueChanged += Box_AfterSimCode_FuncName_ValueChange;
            Box_InternalState_Length.ValueChanged += Box_InternalState_Length_ValueChange;
            Box_ClickType.ValueChanged += Box_ClickType_ValueChanged;
            ComponentValueInputCount.ValueChanged += InputCount_ValueChanged;
            //Box_IsAfterSim.ValueChanged += Box_IsAfterSim_ValueChange;

            // Code Boxes
            CodeBox_Sim = new UI_TextBox(new Pos(0), new Point(250, 400), UI_Handler.gen_conf);
            CodeBox_AfterSim = new UI_TextBox(new Pos(0), new Point(250, 400), UI_Handler.gen_conf);
            CodeBox_Sim.LostFocus += Code_Sim_LostFocus;
            CodeBox_AfterSim.LostFocus += Code_AfterSim_LostFocus;
            UpdatePos();
            Resize();
            GetsUpdated = GetsDrawn = false;


        }

        public void SetRootComp(CompData comp)
        {
            rootcomp = comp;
            Box_Name.value = comp.name;
            Box_Category.value = comp.catagory;
            Box_SimCode_FuncName.value = comp.Code_Sim_FuncName;
            Box_AfterSimCode_FuncName.value = comp.Code_AfterSim_FuncName;
            Box_InternalState_Length.value = comp.internalstate_length.ToString();
            Box_ClickType.value = rootcomp.IsClickable ? rootcomp.ClickAction_Type.ToString() : "";
            ComponentValueInputCount.value = rootcomp.valuebox_length.ToString();

            CodeBox_Sim.t.Text = comp.Code_Sim;
            CodeBox_AfterSim.t.Text = comp.Code_AfterSim;
            gridpaint.pixel.Clear();
            gridpaint.currot = 0;
            parameterlabels.Clear();
            for(int i = 0; i < rootcomp.parameters.Count(); i++)
                {
                parameterlabels.Add(rootcomp.parameters[i]);
                }
            ComponentValueInputCount.value = rootcomp.parameters.Count().ToString();
            ComponentValueInputCount.MakeValueChanged();
            LoadInputCount();
            RotButtonPressed(rotbuttons[0]);
        }

        public void RotButtonPressed(object sender)
        {
            UI_StringButton cur = sender as UI_StringButton;
            int index = Array.IndexOf(rotbuttons, cur);
            if (cur.IsActivated == false)
            {
                cur.IsActivated = true;
            }
            else
            {
                for (int i = 0; i < rotbuttons.Length; ++i)
                {
                    UI_StringButton curbut = rotbuttons[i];
                    if (i != index)
                        curbut.IsActivated = false;
                }
                gridpaint.currot = index;
                gridpaint.ledsegmentpixel.Clear();
                gridpaint.ledsegmentpixel_pos.Clear();
                for (int i = 0; i < rootcomp.overlaylines.Length; ++i)
                {
                    for (int j = 0; j < rootcomp.overlaylines[i][index].Count; ++j)
                    {
                        Point dir = rootcomp.overlaylines[i][index][j].end - rootcomp.overlaylines[i][index][j].start;
                        int length = 1;
                        Point curpos = rootcomp.overlaylines[i][index][j].start;
                        for (int k = 0; k < length; ++k)
                        {
                            gridpaint.ledsegmentpixel.Add((byte)i);
                            gridpaint.ledsegmentpixel_pos.Add(curpos + gridpaint.Origin);
                        }
                    }
                }
                gridpaint.pixel.Clear();
                gridpaint.pixel.AddRange(rootcomp.data[index]);
                gridpaint.ApplyPixel();
            }

        }
        public void PaintButtonPressed(object sender)
        {
            UI_TexButton cur = sender as UI_TexButton;
            int index = Array.IndexOf(paintbuttons, cur);
            if (cur.IsActivated == false)
                cur.IsActivated = true;
            else
            {
                for (int i = 0; i < paintbuttons.Length; ++i)
                {
                    UI_TexButton curbut = paintbuttons[i];
                    if (i != index)
                        curbut.IsActivated = false;
                }
            }
            gridpaint.curplacetype = index + 1;
        }

        public void PixelChanged()
        {
            rootcomp.ClearAllPixel();
            int max = 0;
            if (gridpaint.ledsegmentpixel.Count > 0)
            {
                max = gridpaint.ledsegmentpixel.Max();
                rootcomp.IsOverlay = true;
            }
            else
            {
                rootcomp.IsOverlay = false;
            }
            rootcomp.InitializeLineOverlays(1 + max);
            bool[,] IsCalc = new bool[gridpaint.GridSize, gridpaint.GridSize];
            for(int i = 0; i < gridpaint.pixel.Count; ++i)
            {
                rootcomp.addData(gridpaint.pixel[i], gridpaint.currot);
            }
            for (int i = 0; i < gridpaint.ledsegmentpixel.Count; ++i)
            {
                Line line = new Line(gridpaint.ledsegmentpixel_pos[i] - gridpaint.Origin, gridpaint.ledsegmentpixel_pos[i] - gridpaint.Origin);
                rootcomp.addOverlayLine(line, 255, gridpaint.ledsegmentpixel[i]);
            }
        }

        public override void ChangedUpdate2True()
        {
            base.ChangedUpdate2True();
        }

        public void BoxName_ValueChange(object sender)
        {
            rootcomp.name = Box_Name.value;
            UI_Handler.LibraryEditWindow.Reload_UI();
        }
        public void BoxCategory_ValueChange(object sender)
        {
            rootcomp.catagory = Box_Category.value;
            
        }
        public void Box_SimCode_FuncName_ValueChange(object sender)
        {
            rootcomp.Code_Sim_FuncName = Box_SimCode_FuncName.value;
        }
        public void Box_AfterSimCode_FuncName_ValueChange(object sender)
        {
            rootcomp.Code_AfterSim_FuncName = Box_AfterSimCode_FuncName.value;
            rootcomp.IsUpdateAfterSim = rootcomp.Code_AfterSim_FuncName.Length > 0;
        }
        public void Box_InternalState_Length_ValueChange(object sender)
        {
            rootcomp.internalstate_length = int.Parse("0" + Box_InternalState_Length.value);
        }
        public void Box_ClickType_ValueChanged(object sender)
        {
            if (Box_ClickType.value.Length > 0)
            {
                rootcomp.IsClickable = true;
                rootcomp.ClickAction_Type = int.Parse(Box_ClickType.value);
            }
            else
                rootcomp.IsClickable = false;
        }

        public void Code_Sim_Button_Pressed(object sender)
        {
            CodeBox_Sim.Show();
        }
        public void Code_AfterSim_Button_Pressed(object sender)
        {
            CodeBox_AfterSim.Show();
        }

        public void Code_Sim_LostFocus(object sender)
        {
            rootcomp.Code_Sim = CodeBox_Sim.t.Text;
        }
        public void Code_AfterSim_LostFocus(object sender)
        {
            rootcomp.Code_AfterSim = CodeBox_AfterSim.t.Text;
        }

        protected override void Resize()
        {
            Features.size = new Point(size.X - bezelsize * 2, size.Y - bezelsize - (Features.pos.Y_abs - absolutpos.Y));
            base.Resize();
        }
        public void LoadInputCount()
        {
            for (int i = 0; i < rootcomp.parameters.Count(); i++)
            {
                inputlist[i].value = parameterlabels[i];
            }
        }
        public void SetParameterLabels(object sender)
        {
            UI_ValueInput  cur = sender as UI_ValueInput;
            int index = inputlist.FindIndex(x => x.value == cur.value);
            rootcomp.parameters[index] = cur.value;
        }
        public void InputCount_ValueChanged(object sender)
        {
            int count = int.Parse("0" + ComponentValueInputCount.value);
            rootcomp.valuebox_length = count;
            for (int i = 0; i < labellist.Count; i++)
            {
                Features.ui_elements.Remove(labellist[i]);
                Features.ui_elements.Remove(inputlist[i]);

            }
            rootcomp.parameters.Clear();
            inputlist.Clear();
            labellist.Clear();
            if (count > 0)
            {
                labellist.Add(new UI_String(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, InputCount_Label), new Point(size.X / 2, (int)(UI_Handler.genbutconf.font.MeasureString("Test").Y)), UI_Handler.genbutconf, "Input 0:"));
                inputlist.Add(new UI_ValueInput(new Pos(5, 0, ORIGIN.TR, ORIGIN.DEFAULT, labellist[0]), new Point(size.X / 2, (int)(UI_Handler.genbutconf.font.MeasureString("Test").Y)), UI_Handler.genbutconf, 3, 12));
                inputlist[0].ValueChanged += SetParameterLabels;
                rootcomp.parameters.Add("");

                Features.Add_UI_Elements(labellist[0]);
                Features.Add_UI_Elements(inputlist[0]);
                for (int i = 1; i < count; i++)
                {
                    UI_String newlabel = new UI_String(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, labellist[i - 1]), new Point(size.X / 2, (int)(UI_Handler.genbutconf.font.MeasureString("Test").Y)), UI_Handler.genbutconf, "Input " + i.ToString() + ":");
                    labellist.Add(newlabel);
                    UI_ValueInput newbox = new UI_ValueInput(new Pos(5, 0, ORIGIN.TR, ORIGIN.DEFAULT, labellist[i]), new Point(size.X / 2, (int)(UI_Handler.genbutconf.font.MeasureString("Test").Y)), UI_Handler.genbutconf, 3, 12);
                    inputlist.Add(newbox);
                    inputlist[i].ValueChanged += SetParameterLabels;
                    rootcomp.parameters.Add("");

                    Features.Add_UI_Elements(newlabel);
                    Features.Add_UI_Elements(newbox);
                }
            }
        }

        public override void UpdateSpecific()
        {
            if (paintbuttons[0].IsHovered)
            {
                UI_Handler.info.values.ui_elements[0].setValue("Place Mid-Gray Body");
                UI_Handler.info.ShowInfo();
            }
            else if (paintbuttons[1].IsHovered)
            {
                UI_Handler.info.values.ui_elements[0].setValue("Place Dark-Gray Body");
                UI_Handler.info.ShowInfo();
            }
            else if (paintbuttons[2].IsHovered)
            {
                UI_Handler.info.values.ui_elements[0].setValue("Place MidGray Body");
                UI_Handler.info.ShowInfo();
            }
            else if (paintbuttons[3].IsHovered)
            {
                UI_Handler.info.values.ui_elements[0].setValue("Place Led Segment");
                UI_Handler.info.ShowInfo();
            }
            else if (paintbuttons[4].IsHovered)
            {
                UI_Handler.info.values.ui_elements[0].setValue("Place Pin");
                UI_Handler.info.ShowInfo();
            }

            base.UpdateSpecific();
        }

        protected override void UpdateAlways()
        {
            bool AllPaintButtonsNotHovered = paintbuttons.All(x => x.IsHovered == false);
            if(AllPaintButtonsNotHovered)
                UI_Handler.info.HideInfo();

            base.UpdateAlways();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);
        }
    }

}

