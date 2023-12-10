using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace fitness
{
    public partial class s : Form
    {
        int pic_number = -1;
        public s()
        {
            InitializeComponent();
        }

        private void btnRoutineNumber_Click(object sender, EventArgs e)
        {
            SQLiteConnection con = new SQLiteConnection(@"data source=C:/Users/johnn/.vscode/cs4620/final/fitness/application.db");
            con.Open();

            string query1 = 
                "SELECT image " +
                "FROM progress " +
                "WHERE routine_number=@rnum " +
                "LIMIT 1";

            SQLiteCommand cmd = new SQLiteCommand(query1, con);
            cmd.Parameters.Add(new SQLiteParameter("@rnum", rNumber.Text)); //spent 5 hours not realizing it autocorrected to btn

            //open the image in the picture box
            if (rNumber.Text.Length > 0)
            {
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // Get the image blob as a byte array
                        byte[] picture = (byte[])reader["image"];

                        // Convert to Bitmap and rotate (not sure if all need rotated but all the test images did)
                        Bitmap bm = new Bitmap(new MemoryStream(picture));
                        bm.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        pictureBox1.Image = bm;

                        pic_number = 0;
                    }
                }
                reader.Close();
            }

            con.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string queryFind = 
                "SELECT COUNT(*) " +
                "FROM routine " +
                "WHERE routine_number=@rnumber";

            SQLiteConnection con = new SQLiteConnection(@"data source=C:/Users/johnn/.vscode/cs4620/final/fitness/application.db");
            con.Open();
            SQLiteCommand cmd = new SQLiteCommand(queryFind, con);
            cmd.Parameters.Add(new SQLiteParameter("@rnumber", rNumber.Text));

            string tmp = "";
            if (rNumber.Text.Length > 0)
            {
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tmp += reader[0].ToString();
                }
                reader.Close();
            }

            if (Int32.TryParse(tmp, out int rNum))
            {
                Console.WriteLine("String parsed.");
            }
            else
            {
                Console.WriteLine("String could not be parsed.");
                return;
            }

            if (rNum > 0)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string queryAdd =
                        "INSERT INTO progress VALUES (@rnum, @pic, datetime('now'))";

                    Bitmap bm = new Bitmap(ofd.FileName);
                    bm.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    pictureBox1.Image = bm;

                    //convert and add to the database
                    cmd.CommandText = queryAdd;

                    FileStream picStream = File.OpenRead(ofd.FileName);
                    byte[] picture = new byte[picStream.Length];
                    picStream.Read(picture, 0, (int)picStream.Length);

                    cmd.Parameters.Add(new SQLiteParameter("@rnum", rNumber.Text));
                    SQLiteParameter pic = new SQLiteParameter("@pic", DbType.Binary);
                    pic.Value = picture;
                    cmd.Parameters.Add(pic);

                    cmd.ExecuteNonQuery();
                }
            }

            con.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            SQLiteConnection con = new SQLiteConnection(@"data source=C:/Users/johnn/.vscode/cs4620/final/fitness/application.db");
            con.Open();

            string query1 = 
                "SELECT COUNT(*) " +
                "FROM progress " +
                "WHERE routine_number=@rnumber";

            string query2 = 
                "SELECT image " +
                "FROM progress " +
                "WHERE routine_number=@rnum " +
                "ORDER BY date ASC " + //need to get the first added to be here (or not)
                "LIMIT 1 OFFSET @picnum";

            SQLiteCommand cmd = new SQLiteCommand(query1, con);
            cmd.Parameters.Add(new SQLiteParameter("@rnumber", rNumber.Text));

            if (rNumber.Text.Length > 0)
            {
                var reader = cmd.ExecuteReader();

                string tmp = "";
                while (reader.Read())
                {
                    tmp += reader[0].ToString();
                }
                reader.Close();

                if (Int32.TryParse(tmp, out int rNum))
                {
                    Console.WriteLine("parsed.");
                }
                else
                {
                    Console.WriteLine("String could not be parsed.");
                }

                if (pic_number + 1 <= rNum)
                {
                    cmd.CommandText = query2;
                    cmd.Parameters.Add(new SQLiteParameter("@rnum", rNumber.Text));
                    cmd.Parameters.Add(new SQLiteParameter("@picnum", pic_number+1));

                    Console.WriteLine(pic_number);
                    var reader2 = cmd.ExecuteReader();
                    if (reader2.HasRows)
                    {
                        Console.WriteLine("has rows");
                        while (reader2.Read())
                        {
                            Console.WriteLine("is read.");
                            // Get the image blob as a byte array
                            byte[] picture = (byte[])reader2["image"];

                            // Convert to Bitmap and rotate (not sure if all need rotated but all the test images did)
                            Bitmap bm = new Bitmap(new MemoryStream(picture));
                            bm.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            pictureBox1.Image = bm;

                            pic_number++;
                        }
                    }
                    reader2.Close();
                }
            }

        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            SQLiteConnection con = new SQLiteConnection(@"data source=C:/Users/johnn/.vscode/cs4620/final/fitness/application.db");
            con.Open();

            string query1 =
                "SELECT COUNT(*) " +
                "FROM progress " +
                "WHERE routine_number=@rnumber";

            string query2 =
                "SELECT image " +
                "FROM progress " +
                "WHERE routine_number=@rnum " +
                "ORDER BY date ASC " + //need to get the first added to be here (or not)
                "LIMIT 1 OFFSET @picnum";

            SQLiteCommand cmd = new SQLiteCommand(query1, con);
            cmd.Parameters.Add(new SQLiteParameter("@rnumber", rNumber.Text));

            if (rNumber.Text.Length > 0)
            {
                var reader = cmd.ExecuteReader();

                string tmp = "";
                while (reader.Read())
                {
                    tmp += reader[0].ToString();
                }
                reader.Close();

                if (Int32.TryParse(tmp, out int rNum))
                {
                    Console.WriteLine("parsed.");
                }
                else
                {
                    Console.WriteLine("String could not be parsed.");
                }

                if (pic_number - 1 >= 0)
                {
                    cmd.CommandText = query2;
                    cmd.Parameters.Add(new SQLiteParameter("@rnum", rNumber.Text));
                    cmd.Parameters.Add(new SQLiteParameter("@picnum", pic_number-1));

                    Console.WriteLine(pic_number + "1");
                    var reader2 = cmd.ExecuteReader();
                    if (reader2.HasRows)
                    {
                        Console.WriteLine(pic_number + "2");
                        while (reader2.Read())
                        {
                            Console.WriteLine(pic_number + "3");
                            // Get the image blob as a byte array
                            byte[] picture = (byte[])reader2["image"];

                            // Convert to Bitmap and rotate (not sure if all need rotated but all the test images did)
                            Bitmap bm = new Bitmap(new MemoryStream(picture));
                            bm.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            pictureBox1.Image = bm;

                            pic_number--;
                            Console.WriteLine(pic_number + "4");
                        }
                    }
                    reader2.Close();
                }
            }

        }
    }
}
