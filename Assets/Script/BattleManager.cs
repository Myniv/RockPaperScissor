using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [SerializeField] State state;
    [SerializeField] GameObject battleResult;
    [SerializeField] TMP_Text battleResultText;
    [SerializeField] Player player1;
    [SerializeField] Player player2;

    enum State
    {
        Preparation,
        Player1Select,
        Player2Select,
        Attacking,
        Damaging,
        Returning,
        BattleOver
    }

    void Update()
    {
        switch (state)
        {
            case State.Preparation:
                player1.Prepare();
                player2.Prepare();

                player1.isBot=false;
                player1.SetPlay(true);
                player2.SetPlay(false);
                state = State.Player1Select;
                break;

            case State.Player1Select:
                if (player1.SelectedCharacter != null)
                {
                    //set Player 2 Play Next
                    player2.isBot=true;
                    player1.SetPlay(false);
                    player2.SetPlay(true);
                    state = State.Player2Select;
                }
                break;

            case State.Player2Select:
                if (player2.SelectedCharacter != null)
                {
                    //Set Player 1 and 2 Attacks
                    player2.SetPlay(false);
                    player1.Attack();
                    player2.Attack();
                    state = State.Attacking;
                }
                break;

            case State.Attacking:
                if (player1.IsAttacking() == false && player2.IsAttacking() == false)
                {
                    //Calculate who take damages
                    //Fungsi yang mengeluarkan 2 buah return (yg ini namanya tupple = mirip array dengan multiple data (bisa int,string,dll))
                    //(Player winner, Player loser) = CalculateBattle(player1,player2);
                    CalculateBattle(player1, player2, out Player winner, out Player loser);
                    if (loser == null)
                    {
                        player1.TakeDamage(player2.SelectedCharacter.AttackPower);
                        player2.TakeDamage(player1.SelectedCharacter.AttackPower);
                    }
                    else
                    {
                        loser.TakeDamage(winner.SelectedCharacter.AttackPower);
                    }
                    
                    //Start damage animation
                    state = State.Damaging;
                }
                break;
            case State.Damaging:
                if (player1.IsDamaging() == false && player1.IsDamaging() == false)
                {
                    //Hapur Character dari Player
                    if (player1.SelectedCharacter.CurrentHP == 0)
                    {
                        player1.Remove(player1.SelectedCharacter);
                    }
                    if (player2.SelectedCharacter.CurrentHP == 0)
                    {
                        player2.Remove(player2.SelectedCharacter);
                    }

                    //Jika Darah belom abis, kembali ke tempat asal
                    if (player1.SelectedCharacter != null)
                    {
                        player1.Return();
                    }
                    if (player2.SelectedCharacter != null)
                    {
                        player2.Return();
                    }
                    state = State.Returning;
                }
                break;

            case State.Returning:
                if (player1.IsReturning() == false && player2.IsReturning() == false)
                {
                    if (player1.CharacterList.Count == 0 && player2.CharacterList.Count == 0)
                    {
                        //Menyalakan Canvas PopUp
                        battleResult.SetActive(true);
                        battleResultText.text = "Battle is Over!\nDRAW";
                        state = State.BattleOver;
                    } else if (player1.CharacterList.Count == 0)
                    {
                        //Menyalakan Canvas PopUp
                        battleResult.SetActive(true);
                        battleResultText.text = "Battle is Over!\nPlayer 2 WIN!";
                        state = State.BattleOver;
                    } else if (player2.CharacterList.Count == 0)
                    {
                        //Menyalakan Canvas PopUp
                        battleResult.SetActive(true);
                        battleResultText.text = "Battle is Over!\nPlayer 1 WIN!";
                        state = State.BattleOver;
                    }
                    else
                    {
                        state = State.Preparation;
                    }
                }
                break;

            case State.BattleOver:
                break;
        }
    }

    private void CalculateBattle(Player player1, Player player2, out Player winner, out Player loser)
    {
        var type1 = player1.SelectedCharacter.Type;
        var type2 = player2.SelectedCharacter.Type;

        if (type1 == CharacterType.Rock && type2 == CharacterType.Paper)
        {
            winner = player2;
            loser = player1;
        }
        else if (type1 == CharacterType.Rock && type2 == CharacterType.Scissor)
        {
            winner = player1;
            loser = player2;
        }
        else if (type1 == CharacterType.Paper && type2 == CharacterType.Rock)
        {
            winner = player1;
            loser = player2;
        }
        else if (type1 == CharacterType.Paper && type2 == CharacterType.Scissor)
        {
            winner = player2;
            loser = player1;
        }
        else if (type1 == CharacterType.Scissor && type2 == CharacterType.Paper)
        {
            winner = player1;
            loser = player2;
        }
        else if (type1 == CharacterType.Scissor && type2 == CharacterType.Rock)
        {
            winner = player2;
            loser = player1;
        }
        else
        {
            winner = null;
            loser = null;
        }

    }

    public void Replay(){
        //Load dengan Scene yang sedang aktive (restart scene)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit(){
        //Load sesuai nama scene
        SceneManager.LoadScene("Main");
    }
}
