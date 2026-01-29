using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public class LineView : MonoBehaviour
{
    const float PERSON_SIZE = 120f;
    public const float QUEUE_WIDTH = 110f;

    Transform parentOutOfLine;
    List<PersonView> personsLine = new List<PersonView>();
    [SerializeField] RectTransform rectTransform;
    [SerializeField] CanvasGroup lineCanvasGroup;
    float totalLineHeight;

    public List<PersonView> PersonsLine { get => personsLine; set => personsLine = value; }

    internal void InitLine(int lineIndex,List<int> peopleList, GameObject linePersonPrefab, Transform newPersonHolder, bool SingleLine)
    {
        lineCanvasGroup.alpha = 0;

        totalLineHeight = peopleList.Count * PERSON_SIZE + 20;

        rectTransform.sizeDelta = new Vector2(QUEUE_WIDTH, totalLineHeight-80f);
        rectTransform.localPosition = new Vector2(lineIndex * QUEUE_WIDTH + 10*lineIndex - (SingleLine?0:5), 0);

        float offset = (-totalLineHeight/2) + 10 + (PERSON_SIZE / 2f)+45;

        for (int i = 0; i < peopleList.Count; i++)
        {
            GameObject person = Instantiate(linePersonPrefab,transform);
            PersonView personView = person.GetComponent<PersonView>();

            Vector2 pos = new Vector2(0, i * PERSON_SIZE/1.25f +  offset);
            personView.InitPerson(peopleList[i], pos,PERSON_SIZE);
            personsLine.Add(personView);
            person.transform.SetAsFirstSibling();
        }

        parentOutOfLine = newPersonHolder;
    }

    public async void ShowLineVisually()
    {
        lineCanvasGroup.DOFade(1, 0.15f).OnComplete(async () =>
        {
            for (int i = 0;i < personsLine.Count;i++)
            {
                personsLine[i].ShowPerson();
                await Task.Delay(50);
            }
        });
    }

    public void HideLineVisually()
    {
        lineCanvasGroup.DOFade(0, 0.25f);
    }

    public int GetNumberOfPeopleInLine()
    {
        return personsLine.Count;
    }

    internal PersonView GetPersonViewByIndex(int i)
    {
        GameObject objectToReturn = transform.GetChild(i).gameObject;
        personsLine.RemoveAt(i);
        objectToReturn.transform.SetParent(parentOutOfLine);        
        return objectToReturn.GetComponent<PersonView>();
    }

    public GameObject GetTopPersonFromQueue()
    {
        int numChild = personsLine.Count;

        //GameObject objectToReturn = transform.GetChild(0).gameObject;
        //personsLine.RemoveAt(0);

        GameObject objectToReturn = transform.GetChild(personsLine.Count-1).gameObject;
        personsLine.RemoveAt(0);

        objectToReturn.transform.SetParent(parentOutOfLine);
        Invoke("MoveAllPersonsToPosition",0.5f);
        return objectToReturn;

    }

    public void AddPersonToLine(GameObject person)
    {
        person.transform.SetParent(transform);
        PersonView personView = person.GetComponent<PersonView>();
        personsLine.Add(personView);


        float target = (-totalLineHeight / 2) + 10 + (PERSON_SIZE / 2f);

        Vector2 posTarget = new Vector2(0, target);

        //personView.PersonRectTransform.DOAnchorPos(posTarget, 0.2f);
        personView.PersonMoveAnchored(posTarget, 0.25f, null);
    }

    internal GameObject RemoveSecondTopPersonFromLine()
    {
        int numChild = personsLine.Count;

        //GameObject objectToReturn = transform.GetChild(1).gameObject;
        //personsLine.RemoveAt(1);

        GameObject objectToReturn = transform.GetChild(personsLine.Count-2).gameObject;
        personsLine.RemoveAt(personsLine.Count-2);

        objectToReturn.transform.SetParent(parentOutOfLine);
        //MoveAllPersonsToPosition();
        return objectToReturn;
    }

    public void MoveAllPersonsToPosition()
    {
        for (int i = 0; i < personsLine.Count; i++)
        {
            //Vector2 pos = new Vector2(0, i * PERSON_SIZE);

            float target = i * PERSON_SIZE/1.25f + (-totalLineHeight / 2) + 10 + (PERSON_SIZE / 2f)+45;

            //personsLine[i].PersonRectTransform.DOAnchorPosY(target, 0.35f).SetEase(Ease.OutSine);

            personsLine[i].PersonMoveAnchoredY(target, 0.35f,null);
        }
    }

    
}
