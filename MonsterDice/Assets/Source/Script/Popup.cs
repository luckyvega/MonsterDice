using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Model;

public enum PopupType
{
	none,
	dialog,
	summon
}

class DialogPopup
{
	private string content;

	public DialogPopup() { }

	public void setContent(string c)
	{
		content = c;
	}

	public string getContent()
	{
		return content;
	}

	public Tuple<int, int> getBorder()
	{
		char[] spliter = { '\n' };
		string[] lines = content.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
		int height = lines.Length;
		int width = 0;
		foreach (string line in lines)
		{
			int len = 0;
			for (int i = 0; i < line.Length; i++)
			{
				if ((short)line[i] > 127)
					len += 2;
				else
					len += 1;
			}
			if (len > width)
				width = len;
		}
		return new Tuple<int, int>(width, height);
	}
}