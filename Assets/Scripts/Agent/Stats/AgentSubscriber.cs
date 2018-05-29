using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AgentAction { Null, Dash, Throw, Punch, GetStunned, Score, GroundSlam };
public class AgentSubscriber{
    private List<AgentAction> m_Notifications = new List<AgentAction>();
    public void Notify(AgentAction action)
    {
        m_Notifications.Add(action);
    }
    
    public bool hasNotifications()
    {
        return m_Notifications.Count > 0;
    }

    public AgentAction GetNextNotification()
    {
        AgentAction ret = AgentAction.Null;
        if (hasNotifications())
        {
            ret = m_Notifications[0];
            m_Notifications.RemoveAt(0);
        }
        return ret;
    }
}
