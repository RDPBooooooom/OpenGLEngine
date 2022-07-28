using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using OpenGL.Game.Components.BasicComponents;

namespace OpenGL.Game.Tests;

public class Tests
{
    private Game _game;

    private List<Guid> _guids = new List<Guid>();

    [SetUp]
    public void Setup()
    {
        _game = Game.Instance;

        for (int i = 0; i < 100000; i++)
        {
            Guid id = Guid.NewGuid();
            _game.AddComponent(new TransformComponent(id));
            _guids.Add(id);
        }
    }

    private TimeSpan Time(Action toTime)
    {
        var timer = Stopwatch.StartNew();
        toTime();
        timer.Stop();
        return timer.Elapsed;
    }
    
    [Test]
    public void FindPerformance()
    {
        Random rand = new Random();
        int num = rand.Next(0, _guids.Count);
        
        Assert.That(Time(()=>_game.FindComponent<TransformComponent>(_guids[num])), Is.LessThanOrEqualTo(TimeSpan.FromSeconds(0.003)));
    }
    
}