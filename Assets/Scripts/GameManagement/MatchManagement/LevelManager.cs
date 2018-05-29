using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LoadType { Reload, Random, Next, Specific }

public class LevelManager {
    public string[] m_ActiveLevels = new string[1] {
        "Level_7"
    };
    private string currentLevel = null;
    public AsyncOperation async = null;
    private bool level_staged = false;

    public LevelManager()
    {
        // copnstructor does nothing
    }

    public string Current_Scene()
    {
        return SceneManager.GetActiveScene().name;
    }
    public IEnumerator BeginLoadNextLevel()
    {
        if (! level_staged)
        {
            int curr_index = -1;
            int next_index = -1;
            for(int i = 0; i < m_ActiveLevels.Length; i++)
            {
                if ((currentLevel == null) || 
                    (m_ActiveLevels[i] == currentLevel))
                {
                    curr_index = i;
                    break;
                }
            }
            if (curr_index >= 0)
            {
                // level has been identified
                next_index = (curr_index + 1) == m_ActiveLevels.Length ? 0 : curr_index + 1;
                async = SceneManager.LoadSceneAsync(m_ActiveLevels[next_index]);
                async.allowSceneActivation = false;
                // Everything Succeeded
                level_staged = true;
                currentLevel = m_ActiveLevels[next_index];
                yield return async;
            }
            else
            {
                Debug.Log("ERROR: Could not match current level [" + currentLevel + "] in 'BeginLoadNextLevel'");
                yield break;
            }
        }
        else
        {
            Debug.Log("ERROR: Tried to 'BeginLoadNextLevel' after one was already requested. Ignored second request");
            yield break;
        }
    }

    public IEnumerator BeginLoadRandomLevel()
    {
        if (!level_staged)
        {
            // pick a random level
            int rand_int = Random.Range(0, m_ActiveLevels.Length);
            // load it
            async = SceneManager.LoadSceneAsync(m_ActiveLevels[rand_int]);
            async.allowSceneActivation = false;
            // Everything succeeded
            level_staged = true;
            currentLevel = m_ActiveLevels[rand_int];
            yield return async;
        }
        else
        {
            Debug.Log("ERROR: Tried to 'BeginLoadRandomLevel' after one was already requested. Ignored second request");
            yield break;
        }
    }

    public IEnumerator BeginLoadSpecificLevel(string level_name)
    {
        if (!level_staged)
        {
            bool level_available = false;
            // Ensure that the level requested is in the array of available levels
            foreach (string lvl in m_ActiveLevels)
            {
                if (lvl == level_name)
                {
                    // level found
                    level_available = true;
                    currentLevel = level_name;
                    break;
                }
            }
            // Check if we were able to find the level
            if (level_available)
            {
                // load the level
                async = SceneManager.LoadSceneAsync(currentLevel);
                async.allowSceneActivation = false;
                // everything succeeded loading level.
                level_staged = true;
                yield return async;
            }
            else
            {
                Debug.Log("ERROR: Tried to 'BeginLoadSpecificLevel' [" + level_name + "] but it is not in the list of available levels");
                Debug.Log("       Available Levels Are:");
                foreach (string lvl in m_ActiveLevels)
                {
                    Debug.Log("       - " + lvl);
                }
                Debug.Log("       Add more levels in the LevelManager class.");
                yield break;
            }
        }
        else
        {
            Debug.Log("ERROR: Tried to 'BeginLoadSpecificLevel' after one was already requested. Ignored second request");
            yield break;
        }
    }

    public IEnumerator BeginLoadSameLevel()
    {
        if (!level_staged)
        {
            //async.allowSceneActivation = false;
            async = SceneManager.LoadSceneAsync(currentLevel);
            // Everything succeeded
            level_staged = true;
            yield return async;
        }
        else
        {
            Debug.Log("ERROR: Tried to 'BeginLoadSameLevel' after one was already requested. Ignored second request");
            yield break;
        }
    }

    public void UnstageLevel()
    {
        level_staged = false;
    }

    public bool LevelStaged()
    {
        return level_staged;
    }

    public void LoadStagedLevel()
    {
        async.allowSceneActivation = true;
        level_staged = false;
    }

    public void LoadMenu()
    {
        level_staged = false;
        currentLevel = null;
        async = null;
        SceneManager.LoadScene("Menu");
    }
}
