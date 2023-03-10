using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class DialogCommandParser
{
	private const string ANIM_START_STRING = "<anim:(?<name>(.+?))>";
	private static readonly Regex animStartRegex = new Regex(ANIM_START_STRING); 
	private const string ANIM_END_STRING = "</anim>";
	private static readonly Regex animEndRegex = new Regex(ANIM_END_STRING);

	private const string SPEED_STRING = "<speed:(?<speed>(.+?))>";
	private static readonly Regex speedRegex = new Regex(SPEED_STRING);

	private const string PAUSE_STRING = "<pause:(?<time>(.+?))>";
	private static readonly Regex pauseRegex = new Regex(PAUSE_STRING);

	public static List<DialogCommand> ParseText(string text, out string parsedText) {
		List<DialogCommand> result = new List<DialogCommand>();
		parsedText = text;

		parsedText = HandleAnimStartTags(parsedText, result);
		parsedText = HandleAnimEndTags(parsedText, result);
		parsedText = HandleSpeedTags(parsedText, result);
		parsedText = HandlePauseTags(parsedText, result);

		return result;
	}

	private static string HandleAnimStartTags(string message, List<DialogCommand> result) {
		MatchCollection matches = animStartRegex.Matches(message);

		foreach (Match match in matches)
		{
			result.Add(new DialogCommand
			{
				position = VisibleCharactersUpToIndex(message, match.Index),
				type = DialogCommandType.AnimStart,
				value = match.Groups["name"].Value
			});
		}
		message = Regex.Replace(message, ANIM_START_STRING, "");
		return message;
	}

	private static string HandleAnimEndTags(string message, List<DialogCommand> result) {
		MatchCollection matches = animEndRegex.Matches(message);

		foreach (Match match in matches)
		{
			result.Add(new DialogCommand
			{
				position = VisibleCharactersUpToIndex(message, match.Index),
				type = DialogCommandType.AnimEnd
			});
		}
		message = Regex.Replace(message, ANIM_END_STRING, "");
		return message;
	}

	private static string HandleSpeedTags(string message, List<DialogCommand> result) {
		MatchCollection matches = speedRegex.Matches(message);

		foreach (Match match in matches)
		{
			result.Add(new DialogCommand
			{
				position = VisibleCharactersUpToIndex(message, match.Index),
				type = DialogCommandType.Speed,
				value = match.Groups["speed"].Value
			});
		}

		message = Regex.Replace(message, SPEED_STRING, "");
		return message;
	}

	private static string HandlePauseTags(string message, List<DialogCommand> result) {
		MatchCollection matches = pauseRegex.Matches(message);

		foreach (Match match in matches)
		{
			result.Add(new DialogCommand
			{
				position = VisibleCharactersUpToIndex(message, match.Index),
				type = DialogCommandType.Pause,
				value = match.Groups["time"].Value
			});
		}

		message = Regex.Replace(message, PAUSE_STRING, "");
		return message;
	}

	private static int VisibleCharactersUpToIndex(string message, int index) {
		int result = 0;
		bool insideBrackets = false;
		for (int i = 0; i < index; i++)
		{
			if (message[i] == '<')
			{
				insideBrackets = true;
			}
			else if (message[i] == '>')
			{
				insideBrackets = false;
				result--;
			}
			if (!insideBrackets)
			{
				result++;
			}
			else if (i + 6 < index && message.Substring(i, 6) == "sprite")
			{
				result++;
			}
		}
		return result;
	}
}
