using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GapBuffer
{
    public partial class Form1 : Form
    {
        private string currentFile;
        private char[] str;
        private int bufSize;
        private int bufLeft;
        private int bufRight;
        private int bufSegment;
        private int cursor;

        public Form1()
        {
            InitializeComponent();
            createBuffer();
            textBox1.ContextMenu = new ContextMenu();
            textBox2.ContextMenu = new ContextMenu();
            this.Text = "Безымянный";
        }

        public void createBuffer()
        {
            bufLeft = 0;
            bufSize = 20;
            bufRight = bufSize - 1;
            cursor = bufSize;
            bufSegment = 20;
            str = new char[1000];
            for (int i = 0; i < bufSize; i++)
            {
                str[i] = '_';
            }
            
            textBox2.Text = new string(str);
        }


        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {   
            int ind = textBox1.SelectionStart;
            int length = textBox1.Text.Length;
            char c = e.KeyChar;
            if (c == (char)Keys.Back && (textBox1.SelectionStart!=0 ||textBox1.SelectionLength!=0))
            {
                delete();
               
            }
            if (c == '\r')
            {   if (textBox1.SelectionLength != 0)
                {
                    delete();
                    length -= textBox1.SelectionLength;
                }
                if (ind == length) putLast(Environment.NewLine);
                else putByIndex(Environment.NewLine, ind);
            }
            if (c >= '!' && c <= '~' || c >= 'А' && c <= 'я' || c == ' ' || c == 'Ё' || c == 'ё')
            {
                if (textBox1.SelectionLength != 0)
                {
                    delete();
                    length -= textBox1.SelectionLength;
                }
                if (ind == length) putLast(Convert.ToString(c));
                else putByIndex(Convert.ToString(c),ind);
            }
             
        }

        public void delete()
        {

            if (textBox1.SelectionStart + textBox1.SelectionLength == textBox1.Text.Length && cursor - textBox1.SelectionLength > bufRight)
            {
                deleteLast(textBox1.SelectionLength);
            }
            else
            {
                int index = textBox1.SelectionLength == 0 ? textBox1.SelectionStart - 1 : textBox1.SelectionStart;
                deleteByIndex(index, textBox1.SelectionLength);
            }

        }

        //putLast is a method that puts all of the elements at the end of string;
        private void putLast(string s)
        {
            if (cursor + s.Length >= str.Length)
            {             
                char[] str1=new char[str.Length*2+s.Length];
                Array.Copy(str, str1,str.Length);
                str = str1;                
            }

            for (int i = 0; i < s.Length; i++)
            {
                str[cursor] = s[i];
                cursor++;
            }
            textBox2.Text = new string(str);             
        }

        //deleteLast is a method that deletes the last elements of the string(amount of elements entered by user);
        private void deleteLast(int count)
        {   
            if (count == 0)
            {
                str[cursor - 1] = '\0';
                cursor--;
                if (str[cursor - 1] == '\r')
                {
                    str[cursor - 1] = '\0';
                    cursor--;
                }
            }
            else
            {
                for(int i = 0; i < count; i++)
                {
                    str[cursor - 1] = '\0';
                    cursor--;
                }
            }
            textBox2.Text = new string(str);
        }

        //deleteByIndex is a method that deletes elements by index (index and amount of elements entered by user);
        private void deleteByIndex(int index, int count)
        {
            moveSpace(distance(index));
            if (count == 0)
            {
                if (str[bufRight + 1] == '\r') str[bufRight + 2] = '_';
                str[bufRight + 1] = '_';
                bufRight++;
                bufSize++;
            }
            else
            {
                for (int i = 1; i <= count; i++)
                {
                    str[bufRight + i] = '_';
                }
                bufRight+=count;
                bufSize+=count;
            }
            textBox2.Text = new string(str);
        }

        //putByIndex is a method that puts elements by index(elements and index of insertion entered by user);
        private void putByIndex(string s,int index)
        {   
            moveSpace(distance(index));
            if (s.Length >= bufSize) expanseBuffer(s.Length);
            for(int i = 0; i < s.Length; i++)
            {
                str[bufLeft] = s[i];
                bufLeft++;
                bufSize--;
            }
            textBox2.Text = new string(str);
        }

        //moveSpase is a method that moves the space to certain distance(entered by user);
        private void moveSpace(int distance)
        {   
            if (distance > 0) {
                for(int i = 0; i < distance; i++)
                {
                    str[bufLeft] = str[bufRight + 1];
                    str[bufRight + 1] = '_';
                    bufLeft++;
                    bufRight++;
                }
            }
            else
            {
                distance *= -1;
                for(int i = 0; i < distance; i++)
                {
                    str[bufRight] = str[bufLeft - 1];
                    str[bufLeft - 1] = '_';
                    bufLeft--;
                    bufRight--;
                }
            }
        }

        //distance is a method that calculates where buffer should move around
        private int distance(int index)
        {
            if (bufLeft >= index) index = index - bufLeft;
            else index = index + bufSize - bufRight - 1;
            return index;
        }

        //expanseBuffer is a method that expends buffer on a fixed value of segment, when the buffer is overload;
        private void expanseBuffer(int length)
        {
            bufSegment += length;
            char[] str1 = new char[str.Length + bufSegment];
            for(int i = 0; i < bufLeft; i++)
            {
                str1[i] = str[i];
            }
            for(int i = 0; i < bufSegment; i++)
            {
                str1[bufLeft + i] = '_';
            }
            for(int i = bufLeft; i < cursor; i++)
            {
                str1[i + bufSegment] = str[i];
            }
            str = str1;
            bufRight += bufSegment;
            cursor += bufSegment;
            bufSize += bufSegment;
            bufSegment *= 2;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Copy_Click(object sender, EventArgs e)
        {
            if (textBox1.SelectionLength == 0 || textBox1.Text.Length==0) return;
            Clipboard.SetText(textBox1.Text.Substring(textBox1.SelectionStart, textBox1.SelectionLength));
            textBox1.Focus();
        }

        private void Paste_Click(object sender, EventArgs e)
        {
            int start = textBox1.SelectionStart;

            int length = textBox1.SelectionLength;
            del_Click(sender, e);
            if (start == textBox1.Text.Length)
            {
                putLast(Clipboard.GetText());
                textBox1.Text=textBox1.Text.Insert(start, Clipboard.GetText());
            }
            else
            {
                putByIndex(Clipboard.GetText(), start);
                textBox1.Text=textBox1.Text.Insert(start, Clipboard.GetText());
                
            }
            textBox1.SelectionStart = start + Clipboard.GetText().Length;
            textBox1.Focus();
        }

        private void del_Click(object sender, EventArgs e)
        {
            int start = textBox1.SelectionStart;
            

            if (textBox1.SelectionLength == 0) return;
            if (textBox1.SelectionStart + textBox1.SelectionLength == textBox1.Text.Length && cursor - textBox1.SelectionLength > bufRight)
            {
                
                deleteLast(textBox1.SelectionLength);
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - textBox1.SelectionLength);
                
            }
            else
            {
                int index = textBox1.SelectionLength == 0 ? textBox1.SelectionStart - 1 : textBox1.SelectionStart;
                deleteByIndex(index, textBox1.SelectionLength);
                textBox1.Text = textBox1.Text.Remove(index, textBox1.SelectionLength);

            }
            textBox1.SelectionStart = start;
            textBox1.Focus();
        }

        private void cut_Click(object sender, EventArgs e)
        {
            Copy_Click(sender, e);
            del_Click(sender, e);
        }

        private void создатьToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            bufLeft = 0;
            bufSize = 20;
            bufRight = bufSize - 1;
            cursor = bufSize;
            bufSegment = 4;
            str = new char[100];
            for (int i = 0; i < bufSize; i++)
            {
                str[i] = '_';
            }
            this.Text = "Безымянный";
            textBox1.Text = null;
            textBox2.Text = new string(str);
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            currentFile = openFileDialog1.FileName;
            string text = System.IO.File.ReadAllText(currentFile);
            putLast(text);
            textBox1.Text = text;
            MessageBox.Show("Файл открыт");
            this.Text = openFileDialog1.FileName;
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFile == null)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                    return;
                currentFile = saveFileDialog1.FileName;
                System.IO.File.WriteAllText(currentFile, textBox1.Text);
                MessageBox.Show("Файл сохранен");
            }
            else
            {
                System.IO.File.WriteAllText(currentFile, textBox1.Text);
                MessageBox.Show("Файл сохранен");
            }
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            currentFile = saveFileDialog1.FileName;
            System.IO.File.WriteAllText(currentFile, textBox1.Text);
            MessageBox.Show("Файл сохранен");
            this.Text = saveFileDialog1.FileName;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
