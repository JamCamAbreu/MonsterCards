using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCards {

  static public class Constants {
    public const int CARD_HEIGHT = 8;
    public const int CARD_WIDTH = 25;
    public const int MAX_HAND = 5;
  }

  static public class Utilities {


    static public string getSpaces(int numSpaces) {
      string ret = "";
      for (int i = 0; i < numSpaces; i++) {
        ret += " ";
      }
      return ret;
    }


  }



  // Abstract class:
  public class entity {

    public enum mod {
      close = 0,
      pike,
      range,

      // Move types
      foot,
      mount
    }

    public enum type {
      spearman = 0,
      militia,
      archer,
      scout,


      SIZE  // USED TO GET SIZE OF TYPE ENUM
    }

    public string name;
    public int maxHP;
    public int HP;
    public int pierceArmor;
    public int meleeArmor;
    public int damage;
    public bool active; // whether or not can attack
    public List<mod> mods;
    public List<string> lines;

    // Constructor
    public entity() {
      name = "err";
      active = false;
      maxHP = 1;
      HP = maxHP;
      pierceArmor = 0;
      meleeArmor = 0;
      damage = 0;
      mods = new List<mod>();
      lines = new List<string>();

    } // end constructor

    public string getSpaces(int curLen) {
      int need = Constants.CARD_WIDTH - 1; // subtract right edge
      need -= curLen;
      string ret = "";
      for (int i = 0; i < need; i++) { ret += " "; }
      ret += "|";
      return ret;
    }

    public void updateLines() {
      lines.Clear();
      string curLine;

      var edge = "+-----------------------+"; // 25 chars
      lines.Add(edge);

      curLine = "| " + name;
      lines.Add(curLine + getSpaces(curLine.Length));

      curLine = "| HP: " + HP.ToString() + "/" + maxHP.ToString();
      lines.Add(curLine + getSpaces(curLine.Length));

      curLine = "| Attack: " + damage.ToString();
      lines.Add(curLine + getSpaces(curLine.Length));

      curLine = "| Armor: " + meleeArmor.ToString() + "/" + pierceArmor.ToString();
      lines.Add(curLine + getSpaces(curLine.Length));

      string empty = "|                       |"; // 25 chars
      while (lines.Count < Constants.CARD_HEIGHT - 1) {
        lines.Add(empty);
      }

      lines.Add(edge);
    }

    // Attacking & Defending
    public virtual void attack(entity enemy) { }
    public int getDamage(int dam, int arm) {
      int d = dam - arm;
      if (d < 1) return 1;
      else return d;
    }

  } // end entity abstract class


  public class spearman : entity {
    public spearman() {
      name = "spearman";
      maxHP = 45;
      HP = maxHP;
      damage = 4;
      pierceArmor = 0;
      meleeArmor = 0;

      mods.Add(mod.foot);
      mods.Add(mod.pike);

      updateLines();
    }

    public override void attack(entity enemy) {
      // Calculate
      int bonus = 0;
      if (enemy.mods.Contains(mod.mount)) { bonus += 15; }
      int d = getDamage(damage + bonus, enemy.meleeArmor);

      // Apply damage
      enemy.HP -= d;
      if (enemy.HP < 0) enemy.HP = 0;
    }
  }

  public class archer : entity {
    public archer() {
      name = "archer";
      maxHP = 30;
      HP = maxHP;
      damage = 3;
      pierceArmor = 0;
      meleeArmor = 0;

      mods.Add(mod.foot);
      mods.Add(mod.range);

      updateLines();
    }

    public override void attack(entity enemy) {
      // Calculate
      int bonus = 0;
      if (enemy.mods.Contains(mod.pike)) { bonus += 5; }
      int d = getDamage(damage + bonus, enemy.pierceArmor);

      // Apply damage
      enemy.HP -= d;
      if (enemy.HP < 0) enemy.HP = 0;
    }
  }


  public class militia : entity {
    public militia() {
      name = "militia";
      maxHP = 40;
      HP = maxHP;
      damage = 4;
      pierceArmor = 0;
      meleeArmor = 2;

      mods.Add(mod.foot);
      mods.Add(mod.close);

      updateLines();
    }

    public override void attack(entity enemy) {
      // Calculate
      int bonus = 0;
      //if (enemy.mods.Contains(mod.range)) { bonus += 5; } // needs bonus?
      int d = getDamage(damage + bonus, enemy.meleeArmor);

      // Apply damage
      enemy.HP -= d;
      if (enemy.HP < 0) enemy.HP = 0;
    }
  }


  public class scout : entity {
    public scout() {
      name = "scout";
      maxHP = 45;
      HP = maxHP;
      damage = 3;
      pierceArmor = 2;
      meleeArmor = 0;

      mods.Add(mod.foot);
      mods.Add(mod.close);

      updateLines();
    }

    public override void attack(entity enemy) {
      // Calculate
      int bonus = 0;
      if (enemy.mods.Contains(mod.range)) { bonus += 8; }
      int d = getDamage(damage + bonus, enemy.meleeArmor);

      // Apply damage
      enemy.HP -= d;
      if (enemy.HP < 0) enemy.HP = 0;
    }
  }







  public class player {
    public Queue<entity> deck;
    public List<entity> hand;

    // Constructor:
    public player () {
      deck = new Queue<entity>();
      hand = new List<entity>();
    }

    // Draw cards:
    public void drawCards(int numCards) {
      entity e;
      int i = 0;
      while (i < numCards && deck.Count > 0) {
        e = deck.Dequeue();
        hand.Add(e);
        i++;
      }
    }

    // Discard cards:
    public void discardHand() {
      foreach (entity e in hand) { deck.Enqueue(e); }
      hand.Clear();
    }


    // Add cards:
    public void gainCard(entity.type t) {

      entity c;
      switch (t) {
        case entity.type.archer:
          c = new archer();
          break;

        case entity.type.militia:
          c = new militia();
          break;

        case entity.type.spearman:
          c = new spearman();
          break;

        case entity.type.scout:
          c = new scout();
          break;

        default:
          c = new entity();
          break;
      }

      deck.Enqueue(c);
    }

    // Print lines:
    public void printHand() {
      string curLine;
      int ind = 4 + 4 * (Constants.MAX_HAND - hand.Count);
      string indent = Utilities.getSpaces(ind);

      for (int i = 0; i < Constants.CARD_HEIGHT; i++) {
        curLine = indent; // reset
        for (int j = 0; j < hand.Count; j++) {
          hand[j].updateLines();
          curLine += hand[j].lines[i];
          curLine += indent;
        }

        Console.WriteLine(curLine);
      }

    }

  }



  class Program {


    // ENTRY POINT FOR PROGRAM:
    static void Main(string[] args) {

      Random ran = new Random();

      player p = new player();
      for (int i = 0; i < 20; i++) {
        int t = ran.Next(0, (int)entity.type.SIZE - 1);
        p.gainCard((entity.type)t); // casting from int to card type
      }

      p.drawCards(3);
      p.printHand();
      p.discardHand();
      Console.WriteLine("\n\n");

      p.drawCards(5);
      p.printHand();
      p.discardHand();
      Console.WriteLine("\n\n");

      p.drawCards(1);
      p.printHand();
      p.discardHand();
      Console.WriteLine("\n\n");

      // End of program
      Console.WriteLine("Press any key to exit");
      Console.ReadKey();
    }


  }
}
