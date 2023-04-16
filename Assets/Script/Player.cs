using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [SerializeField] Character selectedCharacter;
    [SerializeField] List<Character> characterList;
    [SerializeField] Transform atkRef;

    //Donot destroy audio manager (jika memakai audio manager yang tidak terhapus)
    // [SerializeField] AudioClip damageClip;
    public bool isBot;
    [SerializeField] UnityEvent onTakeDamage;
    [SerializeField] UnityEvent onRemoving;

    public Character SelectedCharacter { get => selectedCharacter; }
    public List<Character> CharacterList { get => characterList; }


    private void Start()
    {
        if (isBot)
        {
            foreach (var character in CharacterList)
            {
                character.Button.interactable = false;
            }

        }
    }

    public void Prepare()
    {
        selectedCharacter = null;
    }
    public void SelectCharacter(Character character)
    {
        selectedCharacter = character;
    }

    public void SetPlay(bool value)
    {
        if (isBot)
        {
            List<Character> lotteryList = new List<Character>();
            foreach (var character in CharacterList)
            {
                //Ceil otomatis pembulatan keatas walaupun nilai dibawah 0.5 dan diatas 0 || Cont = 0.1 di Ceilkan menjadi 1;
                //Round pembulatan, jika nilai<0.5 dibulatkan menjadi 0 dan nilai > 0.5 dibulatkan menjadi 1
                //Floor otomatis pembulatan kebawah walaupun nilai dibawah 1 dan diatas 0.5 || Cont = 0.99 di floorkan menjadi 0;
                int ticket = Mathf.CeilToInt(((float)character.CurrentHP / (float)character.MaxHP) * 10);
                for (int i = 0; i < ticket; i++)
                {
                    lotteryList.Add(character);
                }
            }
            //Jika hanya Random.Range tetapi memakai #unity Sytstem maka akan berdampak error ke program tetapi
            //jika memakai system dan ingin membuat random range, maka didepan random harus memakai UnityEngine agar tidak error;
            int index = UnityEngine.Random.Range(0, lotteryList.Count);
            selectedCharacter = lotteryList[index];
        }
        else
        {
            foreach (var character in CharacterList)
            {
                character.Button.interactable = value;
            }
        }
    }

    // Cara Rumit yang tidak dipakai //
    /* public void Update()
    {
        if (direction == Vector3.zero)
        {
            return;
        }

        if (Vector3.Distance(selectedCharacter.transform.position, atkRef.position) > 0.1f)
        {
            selectedCharacter.transform.position += direction * Time.deltaTime;

        }
        else
        {
            direction = Vector3.zero;
            selectedCharacter.transform.position = atkRef.position;
        }
    }

    Vector3 direction = Vector3.zero; */
    public void Attack()
    {
        /* direction = atkRef.position - selectedCharacter.transform.position; */

        selectedCharacter.transform
            .DOMove(atkRef.position, 1f, true);

            //untuk tweening (nyerang patah patah)
        // selectedCharacter.transform
        //     .DOMove(atkRef.position, 1f, true)
        //     .SetEase(Ease.InFlash);

    }

    public bool IsAttacking()
    {
        if (selectedCharacter == null)
        {
            return false;
        }
        return DOTween.IsTweening(selectedCharacter.transform, true);
    }

    public void TakeDamage(int damageValue)
    {
        selectedCharacter.ChangeHP(-damageValue);
        var spriteRend = selectedCharacter.GetComponent<SpriteRenderer>();
        spriteRend.DOColor(Color.red, 0.1f).SetLoops(6, LoopType.Yoyo);
        // audioManager.PlaySFX(damageClip);
        onTakeDamage.Invoke();

        //Donot destroy audio manager (jika memakai audio manager yang tidak terhapus)
        // AudioSource.sfxInstance.PlaySFX(damageClip);
    }

    public bool IsDamaging()
    {
        if (selectedCharacter == null)
        {
            return false;
        }
        var spriteRend = selectedCharacter.GetComponent<SpriteRenderer>();
        return DOTween.IsTweening(spriteRend);
    }

    public void Remove(Character character)
    {
        if (CharacterList.Contains(character) == false)
        {
            return;
        }
        if (selectedCharacter == character)
        {
            selectedCharacter = null;
        }

        onRemoving.Invoke();
        character.Button.interactable = false;
        character.gameObject.SetActive(false);
        characterList.Remove(character);
    }

    public void Return()
    {
        selectedCharacter.transform.DOMove(selectedCharacter.InitialPosition, 1f);
    }


    public bool IsReturning()
    {
        if (selectedCharacter == null)
        {
            return false;
        }
        return DOTween.IsTweening(selectedCharacter.transform);
    }

}
