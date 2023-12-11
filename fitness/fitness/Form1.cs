using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.SQLite;
//using System.Data.SqlClient;

namespace fitness
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Sets up the objects in the CheckedListBox.
            string[] muscleGroups = { 
                "biceps", 
                "triceps", 
                "trapezius (traps)",
                "deltoid (delts)",
                "anterior deltoid - front",
                "posterior deltoid - rear",
                "lateral deltoid - side",
                "rotator cuff (shoulder)",
                "erector spinae (mid back)",
                "pectoral (chest)",
                "pectoral - upper",
                "pectoral - lower",
                "latissimus dorsi (lats)",
                "rhomboid (upper back)",
                "abdominal (abs)",
                "quadriceps (quads)",
                "gluteal (glutes)",
                "hamstrings",
                "adductor",
                "abductor",
                "gastrocnemius/soleus (calf)",
                "forearm"
            };
            checkedListBox1.Items.AddRange(muscleGroups);

            // Changes the selection mode from double-click to single click.
            checkedListBox1.CheckOnClick = true;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ///connection object
            string applicationDirectory = System.Windows.Forms.Application.ExecutablePath;
            Console.WriteLine(applicationDirectory);
            SQLiteConnection con = new SQLiteConnection(@"data source=" + applicationDirectory + "/../../../../application.db");

            con.Open();
            ///command object
            string query1 = "SELECT routine_number, routine_name FROM routine";
            SQLiteCommand cmd = new SQLiteCommand(query1, con);
            ///adapter
            

            ///datatable
            DataTable dt = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;

            con.Close();

            //hiding media player if open
            if (axWindowsMediaPlayer1.Visible)
            {
                axWindowsMediaPlayer1.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ///connection object
            string applicationDirectory = System.Windows.Forms.Application.ExecutablePath;
            Console.WriteLine(applicationDirectory);
            SQLiteConnection con = new SQLiteConnection(@"data source=" + applicationDirectory + "/../../../../application.db");
            con.Open();
            ///command object
            string query1 = 
                "SELECT lift_name, description_l " +
                "FROM lift " +
                "WHERE true"; 
            SQLiteCommand cmd = new SQLiteCommand(query1, con);
            ///adapter
            

            ///datatable
            DataTable dt = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;

            con.Close();

            //hiding media player if open
            if (axWindowsMediaPlayer1.Visible)
            {
                axWindowsMediaPlayer1.Visible = false;
            }

        }

        private void rsBtn_Click(object sender, EventArgs e)
        {
            //perform 1 of the 3 queries based on the comboBox2 id number
            string query1 = "";

            //gets the id of the selected drop down option
            comboBox2.ValueMember = "ID";
            int id = (int)comboBox2.SelectedValue;

            //sqlite conection
            string applicationDirectory = System.Windows.Forms.Application.ExecutablePath;
            Console.WriteLine(applicationDirectory);
            SQLiteConnection con = new SQLiteConnection(@"data source=" + applicationDirectory + "/../../../../application.db");
            con.Open();

            switch (id)
            {
                case 1:
                    query1 = 
                        "SELECT lift_name, weight, repetitions " +
                        "FROM is_in " +
                        "WHERE routine_num=@tb1"; //should change these to prevent injection
                    
                    break;

                case 2:
                    query1 =
                        "SELECT routine_name, routine_number " +
                        "FROM routine " +
                        "WHERE routine_name LIKE @tb1";
                        //"WHERE routine_name LIKE '%" +textBox1.Text+"%'";
                    break;

                case 3:
                    query1 = 
                        "SELECT routine_number, routine_name " +
                        "FROM is_in, routine " +
                        "WHERE routine_num!=@tb1 AND routine_num=routine_number AND lift_name IN ( " +
                        "    SELECT DISTINCT lift_name " +
                        "    FROM muscle_group " +
                        "    WHERE group_name IN ( " +
                        "        SELECT group_name " +
                        "        FROM muscle_group, is_in " +
                        "        WHERE muscle_group.lift_name=is_in.lift_name AND routine_num=@tb1 " +
                        "    ) " +
                        ")";
                    break;

                default:
                    break;
            }

            SQLiteCommand cmd = new SQLiteCommand(query1, con);

            if (id == 1 || id == 3)
            {
                string len = textBox1.Text;
                if(!Int32.TryParse(len, out int rNum))
                {
                    return;
                }
                else
                {
                    cmd.Parameters.Add(new SQLiteParameter("@tb1", textBox1.Text));
                }
            }
            else if (id == 2)
            {
                cmd.Parameters.Add(new SQLiteParameter("@tb1", "%" + textBox1.Text + "%"));
            }

            DataTable dt = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;

            con.Close();

            //hiding media player if open
            if (axWindowsMediaPlayer1.Visible)
            {
                axWindowsMediaPlayer1.Visible = false;
            }
        }

        private void lsBtn_Click(object sender, EventArgs e)
        {
            //perform 1 of the 3 queries based on the comboBox1 id number
            string query1 = "";

            //gets the id of the selected drop down option
            comboBox1.ValueMember = "ID";
            int id = (int)comboBox1.SelectedValue;

            //sqlite conection
            string applicationDirectory = System.Windows.Forms.Application.ExecutablePath;
            Console.WriteLine(applicationDirectory);
            SQLiteConnection con = new SQLiteConnection(@"data source=" + applicationDirectory + "/../../../../application.db");
            con.Open();

            switch (id)
            {
                case 1:
                    query1 = 
                        "SELECT group_name " +
                        "FROM muscle_group " +
                        "WHERE lift_name=@tb2"; //should change these to prevent injection
                    break;

                case 2:
                    query1 = 
                        "SELECT routine_number, routine_name " +
                        "FROM is_in, routine " +
                        "WHERE lift_name=@tb2 AND routine_num=routine_number " +
                        "ORDER BY routine_number DESC";
                    break;

                case 3:
                    query1 = 
                        "SELECT DISTINCT lift_name " +
                        "FROM muscle_group " +
                        "WHERE lift_name!=@tb2 AND group_name IN (" +
                        "    SELECT group_name" +
                        "    FROM muscle_group" +
                        "    WHERE lift_name=@tb2" +
                        ")";
                    break;

                default:
                    break;
            }

            SQLiteCommand cmd = new SQLiteCommand(query1, con);
            cmd.Parameters.Add(new SQLiteParameter("@tb2", textBox2.Text));

            DataTable dt = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;

            if (id == 1)
            {
                axWindowsMediaPlayer1.Visible = true;
                //add other media player stuff here 

                string tmp =
                    "SELECT video " +
                    "FROM lift " +
                    "WHERE lift_name=@tb2";

                cmd.CommandText = tmp;
                var reader = cmd.ExecuteReader();

                string video = "";
                while (reader.Read())
                {
                    video += reader[0].ToString();
                }
                reader.Close();

                axWindowsMediaPlayer1.URL = video;
            }
            else
            {
                axWindowsMediaPlayer1.Visible= false;
            }

            con.Close();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = new ComboItem[] {
            new ComboItem{ ID = 1, Text = "Display Lift Details" },//add pictures later
            new ComboItem{ ID = 2, Text = "Show routines with this lift" },
            new ComboItem{ ID = 3, Text = "See other lifts with the same muscle group" }
            };

            comboBox2.DataSource = new ComboItem[] {
            new ComboItem{ ID = 1, Text = "Display the full routine" },
            new ComboItem{ ID = 2, Text = "Show routines containing the keyword" },
            new ComboItem{ ID = 3, Text = "See other routines with similar muscle groups" }
            };
        }

        private void addRoutineBtn_Click(object sender, EventArgs e)
        {
            ///connection object
            string applicationDirectory = System.Windows.Forms.Application.ExecutablePath;
            Console.WriteLine(applicationDirectory);
            SQLiteConnection con = new SQLiteConnection(@"data source=" + applicationDirectory + "/../../../../application.db");
            con.Open();
            
            string queryMax = 
                "SELECT MAX(routine_number) " +
                "FROM routine " +
                "WHERE TRUE";

            ///gets the next available routine number
            SQLiteCommand cmd = new SQLiteCommand(queryMax, con);
            var reader = cmd.ExecuteReader();

            string tmp = "";
            while (reader.Read())
            {
                tmp += reader[0].ToString();
            }
            reader.Close();

            if (Int32.TryParse(tmp, out int rNum))
            {
                rNum++;
                //Console.WriteLine(rNum);
            }
            else
            {
                Console.WriteLine("String could not be parsed.");
            }

            string queryAdd =
                "INSERT INTO routine VALUES (" +
                + rNum + ", @rname, @rdes" +
                ")";

            cmd.Parameters.Add(new SQLiteParameter("@rname", newRoutineName.Text));
            cmd.Parameters.Add(new SQLiteParameter("@rdes", newRoutineDes.Text));

            cmd.CommandText = queryAdd;
            cmd.ExecuteNonQuery();

            con.Close();

            //hiding media player if open
            if (axWindowsMediaPlayer1.Visible)
            {
                axWindowsMediaPlayer1.Visible = false;
            }
        }

        private void addIsInBtn_Click(object sender, EventArgs e)
        {
            ///connection object
            string applicationDirectory = System.Windows.Forms.Application.ExecutablePath;
            Console.WriteLine(applicationDirectory);
            SQLiteConnection con = new SQLiteConnection(@"data source=" + applicationDirectory + "/../../../../application.db");
            con.Open();

            string queryAdd = "";

            if(weightText.Text.Length > 0 && repText.Text.Length > 0)
            {
                queryAdd =
                "INSERT INTO is_in VALUES ( " +
                "@lift, @routine, @weight, @rep " +
                ")";
            }
            else if(weightText.Text.Length > 0 && repText.Text.Length == 0)
            {
                queryAdd =
                "INSERT INTO is_in VALUES ( " +
                "@lift, @routine, @weight, 0 " +
                ")";
            }
            else if (weightText.Text.Length == 0 && repText.Text.Length > 0)
            {
                queryAdd =
                "INSERT INTO is_in VALUES ( " +
                "@lift, @routine, 0, @rep " +
                ")";
            }
            else
            {
                queryAdd =
                "INSERT INTO is_in VALUES ( " +
                "@lift, @rep, 0, 0 " +
                ")";
            }

            SQLiteCommand cmd = new SQLiteCommand(queryAdd, con);
            cmd.Parameters.Add(new SQLiteParameter("@lift", isInLift.Text));
            cmd.Parameters.Add(new SQLiteParameter("@routine", isInRoutine.Text));
            cmd.Parameters.Add(new SQLiteParameter("@weight", weightText.Text));
            cmd.Parameters.Add(new SQLiteParameter("@rep", repText.Text));

            cmd.ExecuteNonQuery();

            con.Close();

            //hiding media player if open
            if (axWindowsMediaPlayer1.Visible)
            {
                axWindowsMediaPlayer1.Visible = false;
            }
        }

        private void addLiftBtn_Click(object sender, EventArgs e)
        {
            //add to lift and add to muscle_group
            ///connection object
            string applicationDirectory = System.Windows.Forms.Application.ExecutablePath;
            Console.WriteLine(applicationDirectory);
            SQLiteConnection con = new SQLiteConnection(@"data source=" + applicationDirectory + "/../../../../application.db");
            con.Open();
            
            string queryAdd = "";
            if(newLiftName.Text.Length > 0)
            {
                if (newLiftDes.Text.Length > 0)
                {
                    if (newVideo.Text.Length > 0)
                    {
                        queryAdd =
                        "INSERT INTO lift VALUES ( " +
                        "@liftName, @liftDes, @Video " +
                        ")";
                    }
                    queryAdd =
                        "INSERT INTO lift VALUES ( " +
                        "@liftName, @liftDes, null " +
                        ")";
                }
                else
                {
                    queryAdd =
                        "INSERT INTO lift VALUES (" +
                        "@liftName, null, null" +
                        ")";
                }
            }
            else
            {
                return;
            }
            
            SQLiteCommand cmd = new SQLiteCommand(queryAdd, con);
            cmd.Parameters.Add(new SQLiteParameter("@liftName", newLiftName.Text));
            cmd.Parameters.Add(new SQLiteParameter("@liftDes", newLiftDes.Text));
            cmd.Parameters.Add(new SQLiteParameter("@Video", newVideo.Text));

            cmd.ExecuteNonQuery();
            
            //add from the checked box
            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
                //add each muscle group
                queryAdd =
                    "INSERT INTO muscle_group VALUES ( " +
                    "@liftName, @groups " +
                    ")";
                cmd.CommandText = queryAdd;
                cmd.Parameters.Add(new SQLiteParameter("@groups", checkedListBox1.CheckedItems[i].ToString()));
                cmd.ExecuteNonQuery();
            }

            con.Close();

            //hiding media player if open
            if (axWindowsMediaPlayer1.Visible)
            {
                axWindowsMediaPlayer1.Visible = false;
            }
        }

        private void btnPics_Click(object sender, EventArgs e)
        {
            //switch to form2
            s f2 = new s();
            f2.ShowDialog();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string applicationDirectory = System.Windows.Forms.Application.ExecutablePath;
            Console.WriteLine(applicationDirectory);
            SQLiteConnection con = new SQLiteConnection(@"data source=" + applicationDirectory + "/../../../../application.db");
            con.Open();

            string queryUpdate = "";
            string queryUpdate2 = "";
            string queryDelete = "";
            SQLiteCommand cmd = new SQLiteCommand(queryUpdate, con);
            if (newLiftName.Text.Length > 0)
            {
                if (newLiftDes.Text.Length > 0)
                {
                    if (newVideo.Text.Length > 0)
                    {
                        queryUpdate =
                        "UPDATE lift " +
                        "SET description_l=@des, video=@path " +
                        "WHERE lift_name=@lname";

                        if (checkedListBox1.CheckedItems.Count > 0)
                        {
                            cmd.CommandText = queryUpdate;
                            cmd.Parameters.Add(new SQLiteParameter("@lname", newLiftName.Text));
                            cmd.Parameters.Add(new SQLiteParameter("@des", newLiftDes.Text));
                            cmd.Parameters.Add(new SQLiteParameter("@path", newVideo.Text));

                            cmd.ExecuteNonQuery();

                            //delete existing
                            queryDelete =
                                    "DELETE FROM muscle_group WHERE lift_name=@lname";
                            cmd.CommandText = queryDelete;
                            cmd.ExecuteNonQuery();

                            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                            {
                                //add each muscle group
                                queryUpdate2 =
                                    "INSERT INTO muscle_group VALUES ( " +
                                    "@lname, @groups " +
                                    ")";
                                cmd.CommandText = queryUpdate2;
                                cmd.Parameters.Add(new SQLiteParameter("@lname", newLiftName.Text));
                                cmd.Parameters.Add(new SQLiteParameter("@groups", checkedListBox1.CheckedItems[i].ToString()));
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            cmd.CommandText = queryUpdate;
                            cmd.Parameters.Add(new SQLiteParameter("@lname", newLiftName.Text));
                            cmd.Parameters.Add(new SQLiteParameter("@des", newLiftDes.Text));
                            cmd.Parameters.Add(new SQLiteParameter("@path", newVideo.Text));

                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        queryUpdate =
                            "UPDATE lift " +
                            "SET description_l=@des " +
                            "WHERE lift_name=@lname";
                        if (checkedListBox1.CheckedItems.Count > 0)
                        {
                            cmd.CommandText = queryUpdate;
                            cmd.Parameters.Add(new SQLiteParameter("@lname", newLiftName.Text));
                            cmd.Parameters.Add(new SQLiteParameter("@des", newLiftDes.Text));
                            cmd.Parameters.Add(new SQLiteParameter("@path", newVideo.Text));

                            cmd.ExecuteNonQuery();

                            //delete existing
                            queryDelete =
                                    "DELETE FROM muscle_group WHERE lift_name=@lname";
                            cmd.CommandText = queryDelete;
                            cmd.ExecuteNonQuery();

                            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                            {
                                //add each muscle group
                                queryUpdate2 =
                                    "INSERT INTO muscle_group VALUES ( " +
                                    "@lname, @groups " +
                                    ")";
                                cmd.CommandText = queryUpdate2;
                                cmd.Parameters.Add(new SQLiteParameter("@lname", newLiftName.Text));
                                cmd.Parameters.Add(new SQLiteParameter("@groups", checkedListBox1.CheckedItems[i].ToString()));
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            cmd.CommandText = queryUpdate;
                            cmd.Parameters.Add(new SQLiteParameter("@lname", newLiftName.Text));
                            cmd.Parameters.Add(new SQLiteParameter("@des", newLiftDes.Text));
                            cmd.Parameters.Add(new SQLiteParameter("@path", newVideo.Text));

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                if(newVideo.Text.Length > 0 && newLiftDes.Text.Length == 0)
                {
                    queryUpdate =
                        "UPDATE lift " +
                        "SET video=@path " +
                        "WHERE lift_name=@lname";
                    if (checkedListBox1.CheckedItems.Count > 0)
                    {
                        cmd.CommandText = queryUpdate;
                        cmd.Parameters.Add(new SQLiteParameter("@lname", newLiftName.Text));
                        cmd.Parameters.Add(new SQLiteParameter("@des", newLiftDes.Text));
                        cmd.Parameters.Add(new SQLiteParameter("@path", newVideo.Text));

                        cmd.ExecuteNonQuery();

                        //delete existing
                        queryDelete =
                                "DELETE FROM muscle_group WHERE lift_name=@lname";
                        cmd.CommandText = queryDelete;
                        cmd.ExecuteNonQuery();

                        for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                        {
                            //add each muscle group
                            queryUpdate2 =
                                "INSERT INTO muscle_group VALUES ( " +
                                "@lname, @groups " +
                                ")";
                            cmd.CommandText = queryUpdate2;
                            cmd.Parameters.Add(new SQLiteParameter("@lname", newLiftName.Text));
                            cmd.Parameters.Add(new SQLiteParameter("@groups", checkedListBox1.CheckedItems[i].ToString()));
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        cmd.CommandText = queryUpdate;
                        cmd.Parameters.Add(new SQLiteParameter("@lname", newLiftName.Text));
                        cmd.Parameters.Add(new SQLiteParameter("@des", newLiftDes.Text));
                        cmd.Parameters.Add(new SQLiteParameter("@path", newVideo.Text));

                        cmd.ExecuteNonQuery();
                    }
                }
                if(newLiftDes.Text.Length == 0 && newVideo.Text.Length == 0)
                {
                    //delete existing
                    queryDelete =
                            "DELETE FROM muscle_group WHERE lift_name=@lname";
                    cmd.CommandText = queryDelete;
                    cmd.Parameters.Add(new SQLiteParameter("@lname", newLiftName.Text));
                    cmd.ExecuteNonQuery();

                    for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                    {
                        //add each muscle group
                        queryUpdate2 =
                            "INSERT INTO muscle_group VALUES ( " +
                            "@lname, @groups " +
                            ")";
                        cmd.CommandText = queryUpdate2;
                        cmd.Parameters.Add(new SQLiteParameter("@lname", newLiftName.Text));
                        cmd.Parameters.Add(new SQLiteParameter("@groups", checkedListBox1.CheckedItems[i].ToString()));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                return;
            }

            con.Close();

            //hiding media player if open
            if (axWindowsMediaPlayer1.Visible)
            {
                axWindowsMediaPlayer1.Visible = false;
            }
        }
    }

    class ComboItem
    {
        public int ID { get; set; }
        public string Text { get; set; }
    }
}
