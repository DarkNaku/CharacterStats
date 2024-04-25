using System.Collections;
using System.Collections.Generic;
using DarkNaku.Stat;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CharacterStatsTest
{
    public enum STAT_TYPE { POWER, HEALTH, INTELIGENCE, WISDOM, AGILITY, LUCKY }

    [Test]
    public void _00_생성_테스트()
    {
        var characterStats = new CharacterStats<STAT_TYPE>("Sample");

        Assert.AreEqual("Sample", characterStats.Name);
    }

    [Test]
    public void _01_스탯_추가_테스트()
    {
        var characterStats = new CharacterStats<STAT_TYPE>("Sample");

        characterStats.AddStat(STAT_TYPE.POWER, 50);
        characterStats.AddStat(STAT_TYPE.HEALTH, 100);
        characterStats.AddStat(STAT_TYPE.INTELIGENCE, 30);

        Assert.AreEqual(3, characterStats.All.Count);

        Assert.AreEqual(50, characterStats[STAT_TYPE.POWER].BaseValue);
        Assert.AreEqual(50, characterStats[STAT_TYPE.POWER].Value);
        
        Assert.AreEqual(100, characterStats[STAT_TYPE.HEALTH].BaseValue);
        Assert.AreEqual(100, characterStats[STAT_TYPE.HEALTH].Value);

        Assert.AreEqual(30, characterStats[STAT_TYPE.INTELIGENCE].BaseValue);
        Assert.AreEqual(30, characterStats[STAT_TYPE.INTELIGENCE].Value);
    }

    [Test]
    public void _02_스탯_수정_테스트()
    {
        var characterStats = new CharacterStats<STAT_TYPE>("Sample");

        characterStats.AddStat(STAT_TYPE.POWER, 50);
        characterStats.AddStat(STAT_TYPE.HEALTH, 100);
        characterStats.AddStat(STAT_TYPE.INTELIGENCE, 30);

        characterStats.AddModifier(STAT_TYPE.POWER, new Modifier(ModifierType.Add, 10));

        Assert.AreEqual(50, characterStats[STAT_TYPE.POWER].BaseValue);
        Assert.AreEqual(60, characterStats[STAT_TYPE.POWER].Value);

        characterStats.AddModifier(STAT_TYPE.HEALTH, new Modifier(ModifierType.Percent, 0.1f));

        Assert.AreEqual(100, characterStats[STAT_TYPE.HEALTH].BaseValue);
        Assert.AreEqual(110, characterStats[STAT_TYPE.HEALTH].Value);
    }

    [Test]
    public void _03_수정_제거_테스트()
    {
        var characterStats = new CharacterStats<STAT_TYPE>("Sample");

        characterStats.AddStat(STAT_TYPE.POWER, 50);
        characterStats.AddStat(STAT_TYPE.HEALTH, 100);
        characterStats.AddStat(STAT_TYPE.INTELIGENCE, 30);

        var key = new object();

        characterStats.AddModifier(STAT_TYPE.POWER, new Modifier(ModifierType.Add, 10) { Source = key });

        Assert.AreEqual(50, characterStats[STAT_TYPE.POWER].BaseValue);
        Assert.AreEqual(60, characterStats[STAT_TYPE.POWER].Value);

        characterStats.AddModifier(STAT_TYPE.HEALTH, new Modifier(ModifierType.Percent, 0.1f).SetID("A"));

        Assert.AreEqual(100, characterStats[STAT_TYPE.HEALTH].BaseValue);
        Assert.AreEqual(110, characterStats[STAT_TYPE.HEALTH].Value);

        characterStats.AddModifier(STAT_TYPE.INTELIGENCE, new Modifier(ModifierType.Multiply, 0.1f) { Source = key });

        Assert.AreEqual(30, characterStats[STAT_TYPE.INTELIGENCE].BaseValue);
        Assert.AreEqual(33, characterStats[STAT_TYPE.INTELIGENCE].Value);

        characterStats.RemoveModifierBySource(key);

        Assert.AreEqual(50, characterStats[STAT_TYPE.POWER].BaseValue);
        Assert.AreEqual(50, characterStats[STAT_TYPE.POWER].Value);

        Assert.AreEqual(100, characterStats[STAT_TYPE.HEALTH].BaseValue);
        Assert.AreEqual(110, characterStats[STAT_TYPE.HEALTH].Value);

        Assert.AreEqual(30, characterStats[STAT_TYPE.INTELIGENCE].BaseValue);
        Assert.AreEqual(30, characterStats[STAT_TYPE.INTELIGENCE].Value);

        characterStats.RemoveModifierByID("A");

        Assert.AreEqual(50, characterStats[STAT_TYPE.POWER].BaseValue);
        Assert.AreEqual(50, characterStats[STAT_TYPE.POWER].Value);

        Assert.AreEqual(100, characterStats[STAT_TYPE.HEALTH].BaseValue);
        Assert.AreEqual(100, characterStats[STAT_TYPE.HEALTH].Value);

        Assert.AreEqual(30, characterStats[STAT_TYPE.INTELIGENCE].BaseValue);
        Assert.AreEqual(30, characterStats[STAT_TYPE.INTELIGENCE].Value);
    }

    [Test]
    public void _04_상속_테스트()
    {
        var parent = new CharacterStats<STAT_TYPE>("Parent");

        parent.AddStat(STAT_TYPE.POWER, 50);
        parent.AddStat(STAT_TYPE.HEALTH, 100);
        parent.AddStat(STAT_TYPE.INTELIGENCE, 30);

        Assert.AreEqual(50, parent[STAT_TYPE.POWER].BaseValue);
        Assert.AreEqual(50, parent[STAT_TYPE.POWER].Value);
        Assert.AreEqual(100, parent[STAT_TYPE.HEALTH].BaseValue);
        Assert.AreEqual(100, parent[STAT_TYPE.HEALTH].Value);
        Assert.AreEqual(30, parent[STAT_TYPE.INTELIGENCE].BaseValue);
        Assert.AreEqual(30, parent[STAT_TYPE.INTELIGENCE].Value);

        var child = new CharacterStats<STAT_TYPE>("Child", parent);

        Assert.AreEqual(50, child[STAT_TYPE.POWER].BaseValue);
        Assert.AreEqual(50, child[STAT_TYPE.POWER].Value);
        Assert.AreEqual(100, child[STAT_TYPE.HEALTH].BaseValue);
        Assert.AreEqual(100, child[STAT_TYPE.HEALTH].Value);
        Assert.AreEqual(30, child[STAT_TYPE.INTELIGENCE].BaseValue);
        Assert.AreEqual(30, child[STAT_TYPE.INTELIGENCE].Value);

        var key = new object();

        parent.AddModifier(STAT_TYPE.POWER, new Modifier(ModifierType.Add, 10) { Source = key });
        parent.AddModifier(STAT_TYPE.HEALTH, new Modifier(ModifierType.Percent, 0.1f) { Source = key });
        parent.AddModifier(STAT_TYPE.INTELIGENCE, new Modifier(ModifierType.Add, 10) { Source = key });

        Assert.AreEqual(50, parent[STAT_TYPE.POWER].BaseValue);
        Assert.AreEqual(60, parent[STAT_TYPE.POWER].Value);
        Assert.AreEqual(100, parent[STAT_TYPE.HEALTH].BaseValue);
        Assert.AreEqual(110, parent[STAT_TYPE.HEALTH].Value);
        Assert.AreEqual(30, parent[STAT_TYPE.INTELIGENCE].BaseValue);
        Assert.AreEqual(40, parent[STAT_TYPE.INTELIGENCE].Value);

        Assert.AreEqual(60, child[STAT_TYPE.POWER].BaseValue);
        Assert.AreEqual(60, child[STAT_TYPE.POWER].Value);
        Assert.AreEqual(110, child[STAT_TYPE.HEALTH].BaseValue);
        Assert.AreEqual(110, child[STAT_TYPE.HEALTH].Value);
        Assert.AreEqual(40, child[STAT_TYPE.INTELIGENCE].BaseValue);
        Assert.AreEqual(40, child[STAT_TYPE.INTELIGENCE].Value);

        child.AddModifier(STAT_TYPE.POWER, new Modifier(ModifierType.Add, 10).SetID("A"));
        child.AddModifier(STAT_TYPE.HEALTH, new Modifier(ModifierType.Add, 10).SetID("A"));
        child.AddModifier(STAT_TYPE.INTELIGENCE, new Modifier(ModifierType.Add, 10).SetID("A"));

        Assert.AreEqual(50, parent[STAT_TYPE.POWER].BaseValue);
        Assert.AreEqual(60, parent[STAT_TYPE.POWER].Value);
        Assert.AreEqual(100, parent[STAT_TYPE.HEALTH].BaseValue);
        Assert.AreEqual(110, parent[STAT_TYPE.HEALTH].Value);
        Assert.AreEqual(30, parent[STAT_TYPE.INTELIGENCE].BaseValue);
        Assert.AreEqual(40, parent[STAT_TYPE.INTELIGENCE].Value);

        Assert.AreEqual(60, child[STAT_TYPE.POWER].BaseValue);
        Assert.AreEqual(70, child[STAT_TYPE.POWER].Value);
        Assert.AreEqual(110, child[STAT_TYPE.HEALTH].BaseValue);
        Assert.AreEqual(120, child[STAT_TYPE.HEALTH].Value);
        Assert.AreEqual(40, child[STAT_TYPE.INTELIGENCE].BaseValue);
        Assert.AreEqual(50, child[STAT_TYPE.INTELIGENCE].Value);

        parent.RemoveModifierBySource(key);

        Assert.AreEqual(50, parent[STAT_TYPE.POWER].BaseValue);
        Assert.AreEqual(50, parent[STAT_TYPE.POWER].Value);
        Assert.AreEqual(100, parent[STAT_TYPE.HEALTH].BaseValue);
        Assert.AreEqual(100, parent[STAT_TYPE.HEALTH].Value);
        Assert.AreEqual(30, parent[STAT_TYPE.INTELIGENCE].BaseValue);
        Assert.AreEqual(30, parent[STAT_TYPE.INTELIGENCE].Value);

        Assert.AreEqual(50, child[STAT_TYPE.POWER].BaseValue);
        Assert.AreEqual(60, child[STAT_TYPE.POWER].Value);
        Assert.AreEqual(100, child[STAT_TYPE.HEALTH].BaseValue);
        Assert.AreEqual(110, child[STAT_TYPE.HEALTH].Value);
        Assert.AreEqual(30, child[STAT_TYPE.INTELIGENCE].BaseValue);
        Assert.AreEqual(40, child[STAT_TYPE.INTELIGENCE].Value);

        child.RemoveModifierByID("A");

        Assert.AreEqual(50, child[STAT_TYPE.POWER].BaseValue);
        Assert.AreEqual(50, child[STAT_TYPE.POWER].Value);
        Assert.AreEqual(100, child[STAT_TYPE.HEALTH].BaseValue);
        Assert.AreEqual(100, child[STAT_TYPE.HEALTH].Value);
        Assert.AreEqual(30, child[STAT_TYPE.INTELIGENCE].BaseValue);
        Assert.AreEqual(30, child[STAT_TYPE.INTELIGENCE].Value);
    }
}