﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using UnityEngine;

public static class Abxr
{
	private static readonly Dictionary<string, DateTime> AssessmentStartTimes = new();
	private static readonly Dictionary<string, DateTime> ObjectiveStartTimes = new();
	private static readonly Dictionary<string, DateTime> InteractionStartTimes = new();
	private static readonly Dictionary<string, DateTime> LevelStartTimes = new();
	
	public enum ResultOptions
	{
		Null,
		Pass,
		Fail,
		Complete,
		Incomplete
	}
	
	public enum InteractionType
	{
		Null,
		Bool,
		Select,
		Text,
		Rating,
		Number
	}

	public enum StoragePolicy
	{
		keepLatest,
		appendHistory
	}

	public enum StorageScope
	{
		device,
		user
	}

    /// <summary>
    /// Add log information at the 'Debug' level
    /// </summary>
    /// <param name="text">The log text</param>
    /// <param name="meta">Any additional information (optional)</param>
    public static void LogDebug(string text, Dictionary<string, string> meta = null)
    {
	    meta ??= new Dictionary<string, string>();
	    meta["sceneName"] = SceneChangeDetector.CurrentSceneName;
	    LogBatcher.Add("debug", text, meta);
    }
    
    /// <summary>
    /// Add log information at the 'Informational' level
    /// </summary>
    /// <param name="text">The log text</param>
    /// <param name="meta">Any additional information (optional)</param>
    public static void LogInfo(string text, Dictionary<string, string> meta = null)
    {
	    meta ??= new Dictionary<string, string>();
	    meta["sceneName"] = SceneChangeDetector.CurrentSceneName;
	    LogBatcher.Add("info", text, meta);
    }
    
    /// <summary>
    /// Add log information at the 'Warning' level
    /// </summary>
    /// <param name="text">The log text</param>
    /// <param name="meta">Any additional information (optional)</param>
    public static void LogWarn(string text, Dictionary<string, string> meta = null)
    {
	    meta ??= new Dictionary<string, string>();
	    meta["sceneName"] = SceneChangeDetector.CurrentSceneName;
	    LogBatcher.Add("warn", text, meta);
    }
    
    /// <summary>
    /// Add log information at the 'Error' level
    /// </summary>
    /// <param name="text">The log text</param>
    /// <param name="meta">Any additional information (optional)</param>
    public static void LogError(string text, Dictionary<string, string> meta = null)
    {
	    meta ??= new Dictionary<string, string>();
	    meta["sceneName"] = SceneChangeDetector.CurrentSceneName;
	    LogBatcher.Add("error", text, meta);
    }
    
    /// <summary>
    /// Add log information at the 'Critical' level
    /// </summary>
    /// <param name="text">The log text</param>
    /// <param name="meta">Any additional information (optional)</param>
    public static void LogCritical(string text, Dictionary<string, string> meta = null)
    {
	    meta ??= new Dictionary<string, string>();
	    meta["sceneName"] = SceneChangeDetector.CurrentSceneName;
	    LogBatcher.Add("critical", text, meta);
    }

    /// <summary>
    /// Add event information
    /// </summary>
    /// <param name="name">Name of the event</param>
    /// <param name="meta">Any additional information (optional)</param>
	public static void Event(string name, Dictionary<string, string> meta = null)
	{
		meta ??= new Dictionary<string, string>();
		meta["sceneName"] = SceneChangeDetector.CurrentSceneName;
		EventBatcher.Add(name, meta);
	}

	/// <summary>
	/// Add event information
	/// </summary>
	/// <param name="name">Name of the event</param>
	/// <param name="position">Adds position tracking of the object</param>
	/// <param name="meta">Any additional information (optional)</param>
	public static void Event(string name, Vector3 position, Dictionary<string, string> meta = null)
	{
		meta ??= new Dictionary<string, string>();
		meta["position_x"] = position.x.ToString();
		meta["position_y"] = position.y.ToString();
		meta["position_z"] = position.z.ToString();
		Event(name, meta);
	}
	
	/// <summary>
	/// Add telemetry information
	/// </summary>
	/// <param name="name">Name of the telemetry</param>
	/// <param name="meta">Any additional information</param>
	public static void TelemetryEntry(string name, Dictionary<string, string> meta)
	{
		meta ??= new Dictionary<string, string>();
		meta["sceneName"] = SceneChangeDetector.CurrentSceneName;
		TelemetryBatcher.Add(name, meta);
	}

	/// <summary>
	/// Get the session data with the default name 'state'
	/// Call this as follows:
	/// StartCoroutine(StorageGetDefaultEntry(scope, result => {
	///	    Debug.Log("Result: " + result);
	/// }));
	/// </summary>
	/// <param name="scope">Get from 'device' or 'user'</param>
	/// <param name="callback">Return value when finished</param>
	/// <returns>All the session data stored under the default name 'state'</returns>
	public static IEnumerator StorageGetDefaultEntry(StorageScope scope, Action<List<Dictionary<string, string>>> callback)
	{
		yield return StorageBatcher.Get("state", scope, callback);
	}

	/// <summary>
	/// Get the session data with the given name
	/// Call this as follows:
	/// StartCoroutine(StorageGetDefaultEntry(scope, result => {
	///	    Debug.Log("Result: " + result);
	/// }));
	/// </summary>
	/// <param name="name">The name of the entry to retrieve</param>
	/// <param name="scope">Get from 'device' or 'user'</param>
	/// <param name="callback">Return value when finished</param>
	/// <returns>All the session data stored under the given name</returns>
	public static IEnumerator StorageGetEntry(string name, StorageScope scope, Action<List<Dictionary<string, string>>> callback)
	{
		yield return StorageBatcher.Get(name, scope, callback);
	}

	/// <summary>
	/// Set the session data with the default name 'state'
	/// </summary>
	/// <param name="entry">The data to store</param>
	/// <param name="scope">Store under 'device' or 'user'</param>
	/// <param name="policy">How should this be stored, 'keep latest' or 'append history' (defaults to 'keep latest')</param>
	public static void StorageSetDefaultEntry(Dictionary<string, string> entry, StorageScope scope, StoragePolicy policy = StoragePolicy.keepLatest)
	{
		StorageBatcher.Add("state", entry, scope, policy);
	}
	
	/// <summary>
	/// Set the session data with the given name
	/// </summary>
	/// <param name="name">The name of the entry to store</param>
	/// <param name="entry">The data to store</param>
	/// <param name="scope">Store under 'device' or 'user'</param>
	/// <param name="policy">How should this be stored, 'keep latest' or 'append history' (defaults to 'keep latest')</param>
	public static void StorageSetEntry(string name, Dictionary<string, string> entry, StorageScope scope, StoragePolicy policy = StoragePolicy.keepLatest)
	{
		StorageBatcher.Add(name, entry, scope, policy);
	}

	/// <summary>
	/// Remove the session data stored under the default name 'state'
	/// </summary>
	/// <param name="scope">Remove from 'device' or 'user' (defaults to 'user')</param>
	public static void StorageRemoveDefaultEntry(StorageScope scope = StorageScope.user)
	{
		CoroutineRunner.Instance.StartCoroutine(StorageBatcher.Delete(scope, "state"));
	}

	/// <summary>
	/// Remove the session data stored under the given name
	/// </summary>
	/// <param name="name">The name of the entry to remove</param>
	/// <param name="scope">Remove from 'device' or 'user' (defaults to 'user')</param>
	public static void StorageRemoveEntry(string name, StorageScope scope = StorageScope.user)
	{
		CoroutineRunner.Instance.StartCoroutine(StorageBatcher.Delete(scope, name));
	}

	/// <summary>
	/// Remove all the session data stored on the device or for the current user
	/// </summary>
	/// <param name="scope">Remove all from 'device' or 'user' (defaults to 'user')</param>
	public static void StorageRemoveMultipleEntries(StorageScope scope = StorageScope.user)
	{
		CoroutineRunner.Instance.StartCoroutine(StorageBatcher.Delete(scope));
	}

	/// <summary>
	/// Send a prompt to the LLM provider
	/// StartCoroutine(AIProxy(prompt, llmProvider, result => {
	///	    Debug.Log("Result: " + result);
	/// }));
	/// </summary>
	/// <param name="prompt">The prompt to send</param>
	/// <param name="llmProvider">The LLM being used</param>
	/// <param name="callback">Return value when finished</param>
	/// <returns>The string returned by the LLM</returns>
	public static IEnumerator AIProxy(string prompt, string llmProvider, Action<string> callback)
	{
		yield return AIProxyApi.SendPrompt(prompt, llmProvider, null, callback);
	}

	///  <summary>
	///  Send a prompt to the LLM provider
	///  StartCoroutine(AIProxy(prompt, llmProvider, result => {
	/// 	    Debug.Log("Result: " + result);
	///  }));
	///  </summary>
	///  <param name="prompt">The prompt to send</param>
	///  <param name="pastMessages">Previous messages sent to the LLM</param>
	///  <param name="llmProvider">The LLM being used</param>
	///  <param name="callback">Return value when finished</param>
	///  <returns>The string returned by the LLM</returns>
	public static IEnumerator AIProxy(string prompt, List<string> pastMessages, string llmProvider, Action<string> callback)
	{
		yield return AIProxyApi.SendPrompt(prompt, llmProvider, pastMessages, callback);
	}

	// Event wrapper functions
	public static void EventAssessmentStart(string assessmentName, Dictionary<string, string> meta = null)
	{
		meta ??= new Dictionary<string, string>();
		meta["verb"] = "started";
		meta["assessment_name"] = assessmentName;
		AssessmentStartTimes[assessmentName] = DateTime.UtcNow;
		Event("assessment_start", meta);
	}
	public static void EventAssessmentComplete(string assessmentName, string score, ResultOptions result = ResultOptions.Complete, Dictionary<string, string> meta = null)
	{
		meta ??= new Dictionary<string, string>();
		meta["verb"] = "completed";
		meta["assessment_name"] = assessmentName;
		meta["score"] = score;
		meta["result_options"] = result.ToString();
		AddDuration(AssessmentStartTimes, assessmentName, meta);
		Event("assessment_complete", meta);
	}
	
	public static void EventObjectiveStart(string objectiveName, Dictionary<string, string> meta = null)
	{
		meta ??= new Dictionary<string, string>();
		meta["verb"] = "started";
		meta["objective_name"] = objectiveName;
		ObjectiveStartTimes[objectiveName] = DateTime.UtcNow;
		Event("objective_start", meta);
	}
	public static void EventObjectiveComplete(string objectiveName, string score, ResultOptions result = ResultOptions.Complete, Dictionary<string, string> meta = null)
	{
		meta ??= new Dictionary<string, string>();
		meta["verb"] = "completed";
		meta["objective_name"] = objectiveName;
		meta["score"] = score;
		meta["result_options"] = result.ToString();
		AddDuration(ObjectiveStartTimes, objectiveName, meta);
		Event("objective_complete", meta);
	}
	
	public static void EventInteractionStart(string interactionName, Dictionary<string, string> meta = null)
    {
	    meta ??= new Dictionary<string, string>();
        meta["verb"] = "started";
        meta["interaction_name"] = interactionName;
        InteractionStartTimes[interactionName] = DateTime.UtcNow;
        Event("interaction_start", meta);
    }
	public static void EventInteractionComplete(string interactionName, string result, string resultDetails = "", InteractionType interactionType = InteractionType.Null, Dictionary<string, string> meta = null)
    {
	    meta ??= new Dictionary<string, string>();
        meta["verb"] = "completed";
        meta["interaction_name"] = interactionName;
        meta["result"] = result;
        meta["result_details"] = resultDetails;
        meta["lms_type"] = interactionType.ToString();
        AddDuration(InteractionStartTimes, interactionName, meta);
        
        // Add assessment_name if there's only one AssessmentStartTimes value
        if (AssessmentStartTimes.Count == 1)
        {
	        meta["assessment_name"] = AssessmentStartTimes.First().Key;
        }
        
        Event("interaction_complete", meta);
    }
	
	public static void EventLevelStart(string levelName, Dictionary<string, string> meta = null)
    {
	    meta ??= new Dictionary<string, string>();
        meta["verb"] = "started";
        meta["level_name"] = levelName;
        LevelStartTimes[levelName] = DateTime.UtcNow;
        Event("level_start", meta);
    }
	public static void EventLevelComplete(string levelName, string score, Dictionary<string, string> meta = null)
    {
	    meta ??= new Dictionary<string, string>();
        meta["verb"] = "completed";
        meta["level_name"] = levelName;
        meta["score"] = score;
        AddDuration(LevelStartTimes, levelName, meta);
        Event("level_complete", meta);
    }

	// ---
	public static void PresentKeyboard(string promptText = null, string keyboardType = null, string emailDomain = null)
	{
		KeyboardHandler.ProcessingSubmit = false;
		if (keyboardType is "text" or null)
		{
			NonNativeKeyboard.Instance.Prompt.text = promptText ?? "Please Enter Your Login";
			NonNativeKeyboard.Instance.PresentKeyboard();
		}
		else if (keyboardType == "assessmentPin")
		{
			NonNativeKeyboard.Instance.Prompt.text = promptText ?? "Enter your 6-digit PIN";
			NonNativeKeyboard.Instance.PresentKeyboard(NonNativeKeyboard.LayoutType.Symbol);
		}
		else if (keyboardType == "email")
		{
			NonNativeKeyboard.Instance.Prompt.text = promptText ?? "Enter your email";
			NonNativeKeyboard.Instance.EmailDomain.text = $"@{emailDomain}";
			NonNativeKeyboard.Instance.PresentKeyboard(NonNativeKeyboard.LayoutType.Email);
		}
	}

	public static void PollUser(string prompt, ExitPollHandler.PollType pollType)
	{
		ExitPollHandler.AddPoll(prompt, pollType);
	}
	
	private static void AddDuration(Dictionary<string, DateTime> startTimes, string name, Dictionary<string, string> meta)
	{
		meta ??= new Dictionary<string, string>();
		if (startTimes.ContainsKey(name))
		{
			double duration = (DateTime.UtcNow - startTimes[name]).TotalSeconds; //TODO do we want seconds?
			meta["duration"] = duration.ToString();
			startTimes.Remove(name);
		}
		else
		{
			meta["duration"] = "0";
		}
	}
}