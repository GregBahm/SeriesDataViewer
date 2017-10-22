using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataLoader
{
    public static string DataSourcePath = Application.dataPath + "\\Simpsons.txt";
    
    public static IEnumerable<EpisodeData> LoadData(TextAsset asset)
    {
        List<EpisodeData> data = new List<EpisodeData>();
        string[] splitFile = asset.text.Split('\n');
        data.AddRange(splitFile.Select(LineToData));
        return data;
    }

    private static EpisodeData LineToData(string line)
    {
        EpisodeData ret = new EpisodeData();
        string[] splitString = line.Split('\t');

        string[] splitSeasonLine = splitString[0].Split('.');
        string seasonString = splitSeasonLine[0];
        string episodeString = splitSeasonLine[1];

        string imdbString = splitString[3];
        string nealsonString = splitString[2].Split('[')[0];
        ret.Title = splitString[1];
        ret.Season = Convert.ToInt32(seasonString);
        ret.Episode = Convert.ToInt32(episodeString);
        ret.ImdbRating = Convert.ToSingle(imdbString);
        if (nealsonString != "N/A")
        {
            ret.NealsonRating = Convert.ToSingle(nealsonString);
        }
        return ret;
    }
}


public class EpisodeData
{
    public string Title;
    public int Season;
    public int Episode;
    public float ImdbRating;
    public float NealsonRating;
}