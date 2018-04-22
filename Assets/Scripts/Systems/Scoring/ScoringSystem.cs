using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ScoringSystem : ComponentSystem
{
    private Text ScoreText;

    private ComponentGroup scoreingGroup;
    private ComponentGroup additiveGroup;
    private ComponentGroup subtractiveGroup;

    protected override void OnCreateManager(int capacity)
    {
        base.OnCreateManager(capacity);

        scoreingGroup = GetComponentGroup(typeof(ScoreKeeper), typeof(ScoringGroup));

        additiveGroup = GetComponentGroup(typeof(AddScore), typeof(ScoringGroup));
        subtractiveGroup = GetComponentGroup(typeof(DeductScore), typeof(ScoringGroup));
    }

    protected override void OnUpdate()
    {
        for (int index = 0; index < 1; index++)
        {
            scoreingGroup.SetFilter(new ScoringGroup() { GroupId = index });

            if (scoreingGroup.CalculateLength() == 0) return;

            int valueToAdd = DoAddition(index);
            int valueToSubtract = DoSubtraction(index);

            if (valueToAdd + valueToSubtract == 0) return;

            var scoreKeepers = scoreingGroup.GetComponentDataArray<ScoreKeeper>();

            for (int i = 0; i < scoreKeepers.Length; i++)
            {
                ScoreKeeper scoreKeeper = scoreKeepers[i];
                scoreKeeper.Score += valueToAdd - valueToSubtract;
                scoreKeepers[i] = scoreKeeper;
            }

            ScoreText.text = scoreKeepers[index].Score.ToString();
        }
    }

    private int DoAddition(int index)
    {
        if (additiveGroup.CalculateLength() == 0) return 0;

        additiveGroup.SetFilter(new ScoringGroup() { GroupId = index });
        var addScores = additiveGroup.GetComponentDataArray<AddScore>();
        int valueToAdd = 0;
        for (int i = 0; i < addScores.Length; i++)
        {
            valueToAdd += addScores[i].Value;
        }

        EntityManager.DestroyEntity(additiveGroup.GetEntityArray().GetChunkArray(0, additiveGroup.CalculateLength()));
        return valueToAdd;
    }

    private int DoSubtraction(int index)
    {
        if (subtractiveGroup.CalculateLength() == 0) return 0;

        subtractiveGroup.SetFilter(new ScoringGroup() { GroupId = index });
        var subtractScores = subtractiveGroup.GetComponentDataArray<DeductScore>();

        int valueToSubtract = 0;
        for (int i = 0; i < subtractScores.Length; i++)
        {
            valueToSubtract += subtractScores[i].Value;
        }
        EntityManager.DestroyEntity(subtractiveGroup.GetEntityArray().GetChunkArray(0, subtractiveGroup.CalculateLength()));

        return valueToSubtract;
    }

    public void SetScoringText(Text scoreText)
    {
        ScoreText = scoreText;
    }
}