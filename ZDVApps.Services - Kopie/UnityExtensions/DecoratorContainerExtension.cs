﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace ZDVApps.Services.UnityExtensions
{
    public class DecoratorContainerExtension :UnityContainerExtension
    {
        private Dictionary<Type, List<Type>> _typeStacks;
      protected override void Initialize()
      {
          _typeStacks = new Dictionary<Type, List<Type>>();
          Context.Registering += AddRegistration;
  
         Context.Strategies.Add(
             new DecoratorBuilder(_typeStacks),
             UnityBuildStage.PreCreation
         );
     }
 
        private void AddRegistration(
         object sender, 
         RegisterEventArgs e)
     {
         if (!e.TypeFrom.IsInterface)
         {
             return;
         }
 
         List<Type> stack = null;
         if (!_typeStacks.ContainsKey(e.TypeFrom))
         {
             stack = new List<Type>();
             _typeStacks.Add(e.TypeFrom, stack);
         }
         else
         {
             stack = _typeStacks[e.TypeFrom];
         }
 
         stack.Add(e.TypeTo);
     }
 }

    }

