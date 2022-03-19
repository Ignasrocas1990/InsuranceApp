/*
    Copyright 2020,Ignas Rocas

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
    
              Name : Ignas Rocas
    Student Number : C00135830
           Purpose : 4th year project
 */
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Insurance_app.Service {
  internal class CardDefinitionService {
    private readonly CardDefinition[] cardDefinitions;

    private readonly CardDefinition error = new CardDefinition {
      Length = 16,
      Image = ImageService.Instance.CardError
    };

    private readonly CardDefinition unknown = new CardDefinition {
      Length = 16,
      Image = ImageService.Instance.CardUnknown
    };

    private CardDefinitionService() {
      cardDefinitions = new[] {

        new CardDefinition {
          Image = ImageService.Instance.Discover,
          Length = 16,
          Prefixes = {60},
          Ranges = {(64, 65)}
        },

        new CardDefinition {
          Image = ImageService.Instance.Jcb,
          Length = 16,
          Ranges = {(35, 36)}
        },

        new CardDefinition {
          Image = ImageService.Instance.Mastercard,
          Length = 16,
          Ranges = {(50, 59), (22, 27)},
          Prefixes = {67}
        },

        new CardDefinition {
          Image = ImageService.Instance.Unionpay,
          Length = 16,
          Prefixes = {62}
        },

        new CardDefinition {
          Image = ImageService.Instance.Visa,
          Length = 16,
          Ranges = {(40, 49)}
        },
        new CardDefinition {
          Image = ImageService.Instance.Visa,
          Length = 13,
          Ranges = {
            (413600, 413600),
            (444509, 444509),
            (444509, 444509),
            (444550, 444550),
            (450603, 450603),
            (450617, 450617),
            (450628, 450629),
            (450636, 450636),
            (450640, 450641),
            (450662, 450662),
            (463100, 463100),
            (476142, 476142),
            (476143, 476143),
            (492901, 492902),
            (492920, 492920),
            (492923, 492923),
            (492928, 492930),
            (492937, 492937),
            (492939, 492939),
            (492960, 492960)
          }
        }
      };
    }

    public static CardDefinitionService Instance { get; } = new CardDefinitionService();

    public (int length, ImageSource image) DetailsFor(string cardNumber) {
      var definition = DefinitionFor(cardNumber);

      return (definition.Length, definition.Image);
    }


    private CardDefinition DefinitionFor(string cardNumber) {
      if (cardNumber.Length == 0) return unknown;

      var twoChars = cardNumber.Length >= 2 ? cardNumber.Substring(0, 2) : null;
      var fourChars = cardNumber.Length >= 4 ? cardNumber.Substring(0, 4) : null;


      var two = 0;
      var four = 0;


      if (twoChars != null) int.TryParse(twoChars, out two);


      if (fourChars != null) int.TryParse(fourChars, out four);

      var prefixMatches = new HashSet<CardDefinition>();

      foreach (var cardDefinition in cardDefinitions) {
        if (cardDefinition.Prefixes.Contains(two) || cardDefinition.Prefixes.Contains(four)) return cardDefinition;

        foreach (var (lower, upper) in cardDefinition.Ranges) {
          if (two >= lower && two <= upper || four >= lower && four <= upper) return cardDefinition;

          if (lower.ToString().StartsWith(cardNumber)) prefixMatches.Add(cardDefinition);
        }

        if (cardDefinition.Prefixes.Any(p => p.ToString().StartsWith(cardNumber))) prefixMatches.Add(cardDefinition);
      }

      var groupedPrefixes = prefixMatches.GroupBy(p => p.Image).ToList();

      switch (groupedPrefixes.Count) {
        case 0:
          return error;
        case 1:
          return groupedPrefixes[0].First();
        default:
          return unknown;
      }
    }
    private class CardDefinition {
      public ImageSource Image { get; set; }
      public int Length { get; set; }

      public List<int> Prefixes { get; } = new List<int>();


      public List<(int lower, int upper)> Ranges { get; } = new List<(int lower, int upper)>();
    }
  }
}