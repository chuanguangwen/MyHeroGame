using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class CsvSplitLines
{
    private string[,] lineData;
    public int column;
    public int row;

    public string Get(int _row, int _column)
    {
        if (_row < row && _column < column)
        {
            return lineData[_row, _column];
        }

        return "";
    }

    public void Init(int _row, int _column)
    {
        row = _row;
        column = _column;

        lineData = new string[row, column];
    }

    public void SetLine(int index, string[] line)
    {
        for (int i = 0; i < line.Length; ++i)
        {
            lineData[index, i] = line[i];
        }
    }
}

public static class BaseHelper
{
    static public Quaternion Inv(this Quaternion q)
    {
        return Quaternion.Inverse(q);
    }

    // read csv file.
    static public CsvSplitLines ReadCsvSplitLines(string filename)
    {
        //string str = ReadStreamingAssetText(filename);
        //if (str.Length == 0)
        //{
        //    Debug.LogError(string.Format("ReadCSV:{0} fail!", filename));
        //    return null;
        //}

        //string[] allLine = str.Split(new string[1] { GameConfig.CSV_LINE }, System.StringSplitOptions.None);

        //string[] cellSplit = new string[1] { GameConfig.CSV_SPLIT };

        //string[] firstLine = allLine [0].Split(cellSplit, System.StringSplitOptions.None);
        //int column = firstLine.Length;
        //if (column == 0)
        //{
        //    Debug.LogError(string.Format("ReadCSV:{0} fail, column = 0!", filename));
        //    return null;
        //}

        //CsvSplitLines output = new CsvSplitLines();
        //output.Init(allLine.Length, column);
        //output.SetLine(0, firstLine);

        //for (int i = 1; i<allLine.Length; ++i)
        //{
        //    string[] line = allLine [i].Split(cellSplit, System.StringSplitOptions.None);
        //    if (line.Length != column)
        //    {
        //        Debug.LogError(string.Format("ReadCSV:{0} column invalid, head={1}, line{2}={3}!",
        //                                     filename, column, i, line.Length));
        //        return null;
        //    }
        //    output.SetLine(i, line);
        //}

        //return output;
        return new CsvSplitLines();
    }



    static public Component GetComponent(GameObject obj, string name)
    {
        if (null != obj)
        {
            return obj.GetComponent(name);
        }

        return null;
    }

    // 删除BOM头
    static private byte[] DeleteBom(byte[] src)
    {
        if (src != null && src.Length > 3)
        {
            if (src[0] == 0xEF && src[1] == 0xBB && src[2] == 0xBF)
            {
                int len = src.Length - 3;
                byte[] dst = new byte[len];
                Array.Copy(src, 3, dst, 0, len);
                return dst;
            }
        }

        return src;
    }

    static public string TrimBom2Utf8(byte[] src)
    {
        return System.Text.Encoding.UTF8.GetString(DeleteBom(src));
    }

    static public void ForceDeleteFile(string filename)
    {
        if (File.Exists(filename))
        {
            FileInfo fi = new FileInfo(filename);
            fi.Attributes = FileAttributes.Normal;

            File.Delete(filename);
        }
    }

    static public void ForceDeleteFiles(string pathname, string filename)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(pathname);
        if (null == dirInfo || !dirInfo.Exists)
        {
            return;
        }

        foreach (FileInfo newInfo in dirInfo.GetFiles(filename))
        {
            ForceDeleteFile(newInfo.FullName);
        }
    }

    static public void DeleteFileByDirectory(string pathname)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(pathname);
        if (null == dirInfo || !dirInfo.Exists)
        {
            return;
        }

        foreach (DirectoryInfo newInfo in dirInfo.GetDirectories())
        {
            DeleteFileByDirectory(newInfo.FullName);
        }

        foreach (FileInfo newInfo in dirInfo.GetFiles())
        {
            ForceDeleteFile(newInfo.FullName);
        }

        dirInfo.Attributes = FileAttributes.Normal;
        dirInfo.Delete();
    }

    static public void ClearReadOnlyByDirectory(string pathname)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(pathname);
        if (null == dirInfo || !dirInfo.Exists)
        {
            return;
        }

        dirInfo.Attributes = FileAttributes.Normal;

        foreach (FileInfo newInfo in dirInfo.GetFiles())
        {
            newInfo.Attributes = FileAttributes.Normal;
        }

        foreach (DirectoryInfo newInfo in dirInfo.GetDirectories())
        {
            ClearReadOnlyByDirectory(newInfo.FullName);
        }
    }

    static public bool MakDirValid(string dir)
    {
        if (string.IsNullOrEmpty(dir))
        {
            Debug.LogError(string.Format("MakeDirValidFail1, Empty"));
            return false;
        }

        if (Directory.Exists(dir))
        {
            return true;
        }

        try
        {
            Directory.CreateDirectory(dir);
        }
        catch (Exception e)
        {
            Debug.LogError(string.Format("MakeDirValidFail2: {0}, {1}", dir, e.ToString()));
            return false;
        }

        return true;
    }
}