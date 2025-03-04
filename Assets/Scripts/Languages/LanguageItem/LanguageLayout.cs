﻿using Languages.LanguageItem.Containers;
using System;
using UnityEngine;
using Utils;
using Utils.UI;

namespace Languages.LanguageItem {
    [RequireComponent(typeof(Canvas))]
    [ExecuteAlways]
    public class LanguageLayout : MonoBehaviour, IFadable {

        [SerializeField] private Canvas uiCanvas;
        [Header("Containers")]
        [SerializeField] private LanguageNameContainer languageName;
        [SerializeField] private YearsContainer years;
        [SerializeField] private MapContainer map;
        [SerializeField] private AlphabetContainer alphabet;
        [SerializeField] private DescriptionContainer description;
        [SerializeField] private ImageContainer picture;

        [Header("Spacing")]
        [SerializeField][Range(0, 1)] private float yearsSpacing;
        [SerializeField][Range(0, 1)] private float alphabetSpacing;
        [SerializeField][Range(0, 1)] private float mapSpacing;
        [SerializeField][Range(0, 1)] private float descriptionSpacing;
        [SerializeField][Range(0, 1)] private float pictureSpacing;

        [SerializeField][HideInInspector] private Camera mainCamera;


        private void Awake() {
            mainCamera = Camera.main;
            uiCanvas = GetComponent<Canvas>();
            BindCameraToCanvas();
            RotateTowardsCamera();

            // Get all container components
            languageName = gameObject.GetLanguageComponent(languageName, "LanguageName");
            years = gameObject.GetLanguageComponent(years, "Years");
            alphabet = gameObject.GetLanguageComponent(alphabet, "Alphabet");
            description = gameObject.GetLanguageComponent(description, "Description");
            map = gameObject.GetLanguageComponent(map, "Map");
            picture = gameObject.GetLanguageComponent(picture, "Picture");
        }

        private void Update() {
            RotateTowardsCamera();
            if (!Application.isPlaying) {
                AlignYears();
                AlignMap();
                AlignAlphabet();
                AlignPicture();
                AlignDescription();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(languageName.transform.position, 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(years.transform.position, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(map.transform.position, 0.1f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(alphabet.transform.position, 0.1f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(description.transform.position, 0.1f);
            Gizmos.color = new Color(1, 0.6f, 0, 1);
            Gizmos.DrawSphere(picture.transform.position, 0.1f);
        }
#endif

        public void ToNode() {
            // move layout to same layer as lines
            gameObject.layer = LayerMask.NameToLayer("Default");
            if (!gameObject.activeInHierarchy) {
                StartCoroutine(((IFadable)languageName).FadeIn(0.5f));
            }

            if (years.gameObject.activeInHierarchy) {
                StartCoroutine(((IFadable)this).FadeOut(0.2f, () => {
                    years.ToNode();
                    map.ToNode();
                    if (!alphabet.IsEmpty()) {
                        alphabet.ToNode();
                    }
                    description.ToNode();
                    picture.ToNode();
                    languageName.ToNode();
                }));
            } else {
                languageName.ToNode();
            }
        }

        public void ToItem() {
            // bring layout in front of lines
            gameObject.layer = LayerMask.NameToLayer("UI");
            if (!gameObject.activeInHierarchy) {
                StartCoroutine(((IFadable)languageName).FadeIn(0.5f));
            }

            // years is just a representative!
            if (!years.gameObject.activeInHierarchy) {
                StartCoroutine(((IFadable)this).FadeIn(0.5f));
            }
            languageName.ToItem();
            years.ToItem();
            map.ToItem();
            alphabet.ToItem();
            description.ToItem();
            picture.ToItem();

            // set BG sizes
            var midWidth = Mathf.Max(
                years.GetSize().x,
                languageName.GetSize().x
                );

            years.SetWidth(midWidth);
            languageName.SetWidth(midWidth);

            AlignYears();
            AlignMap();
            AlignDescription();
            AlignAlphabet();
            AlignPicture();
        }

        public void ToItemRelative() {
            // bring layout in front of lines
            gameObject.layer = LayerMask.NameToLayer("UI");
            if (!gameObject.activeInHierarchy) {
                StartCoroutine(((IFadable)languageName).FadeIn(0.5f));
            }
            if (years.gameObject.activeInHierarchy) {
                StartCoroutine(((IFadable)this).FadeOut(0.2f, () => {
                    languageName.ToItemRelative();
                    years.ToItemRelative();
                    map.ToItemRelative();
                    if (!alphabet.IsEmpty()) {
                        alphabet.ToItemRelative();
                    }
                    description.ToItemRelative();
                    picture.ToItemRelative();
                }));
            } else {
                languageName.ToItemRelative();
            }
        }

        /// <summary>
        /// DEPENDENCIES: LanguageName (position)
        /// </summary>
        private void AlignYears() {
            var topBound = languageName.GetTopRight().y;
            var newPosition = new Vector3(0, topBound + yearsSpacing, 0);
            years.SetLocalPosition(newPosition);
        }

        /// <summary>
        /// DEPENDENCIES: LanguageName (position)
        /// </summary>
        private void AlignMap() {
            var nameBotLeft = languageName.GetBotLeft();
            var newPosition = new Vector3(nameBotLeft.x - mapSpacing, nameBotLeft.y, 0);
            map.SetLocalPosition(newPosition);
        }

        /// <summary>
        /// DEPENDENCIES: LanguageName (size), Map (position)
        /// </summary>
        private void AlignDescription() {
            var mapBotLeft = map.GetBotLeft();
            var newPosition = new Vector3(mapBotLeft.x, mapBotLeft.y - descriptionSpacing, 0);
            description.SetLocalPosition(newPosition);
            var descWidth = Mathf.Abs(languageName.GetTopRight().x - map.GetBotLeft().x);
            description.SetWidth(descWidth);
        }

        /// <summary>
        /// DEPENDENCIES: LanguageName (position), Description (position), Picture (size)
        /// </summary>
        private void AlignAlphabet() {
            if (alphabet.IsEmpty()) {
                return;
            }
            Vector3 newPosition;
            var alphabetHeight = Mathf.Abs(alphabet.GetTopRight().y - alphabet.GetBotLeft().y);
            var pictureHeight = Mathf.Abs(picture.GetTopRight().y - picture.GetBotLeft().y);
            if (picture.IsEmpty() || alphabetHeight >= pictureHeight) {
                var nameTopRight = languageName.GetTopRight();
                newPosition = new Vector3(nameTopRight.x + alphabetSpacing, nameTopRight.y, 0);
            } else {
                var descTopRight = description.GetTopRight();
                newPosition = new Vector3(descTopRight.x + alphabetSpacing, descTopRight.y, 0);
            }
            alphabet.SetLocalPosition(newPosition);
        }

        /// <summary>
        /// DEPENDENCIES: Description (position & size), Alphabet (position)
        /// </summary>
        private void AlignPicture() {
            Vector3 newPosition;
            if (alphabet.IsEmpty()) {
                var descriptionTopRight = description.GetTopRight();
                var descriptionBotLeft = description.GetBotLeft();
                newPosition = new Vector3(descriptionTopRight.x + pictureSpacing, descriptionBotLeft.y, 0);
            } else {
                var alphabetTopRight = alphabet.GetTopRight();
                var alphabetBotLeft = alphabet.GetBotLeft();
                newPosition = new Vector3(alphabetBotLeft.x, alphabetTopRight.y + pictureSpacing, 0);
            }
            picture.SetLocalPosition(newPosition);
        }

        public float GetOpacity() {
            return languageName.GetOpacity();
        }

        public void SetOpacity(float percent) {
            // languageName.SetOpacity(percent);
            years.SetOpacity(percent);
            map.SetOpacity(percent);
            description.SetOpacity(percent);
            alphabet.SetOpacity(percent);
            picture.SetOpacity(percent);
        }

        private void RotateTowardsCamera() {
            if (!mainCamera) {
                return;
            }
            uiCanvas.transform.forward = mainCamera.transform.forward;
            // languageName.RotateTowardsCamera();
        }

        private void BindCameraToCanvas() {
            if (!uiCanvas) {
                uiCanvas = GetComponentInChildren<Canvas>();
            }
            if (!uiCanvas) {
                throw new MissingComponentException("LanguageNode is missing Canvas component!");
            }
            uiCanvas.worldCamera = mainCamera == null ? Camera.main : mainCamera;
        }

        public void SetName(string newName) {
            languageName.SetName(newName);
        }

        public void SetPhonetic(string newPhonetic) {
            languageName.SetPhonetic(newPhonetic);
        }

        public void SetYears(string newYears) {
            years.SetText(newYears);
        }

        public void SetMap(string langName) {
            map.LoadImage(langName, !alphabet.IsEmpty());
        }

        public void SetAlphabet(string newAlphabet) {
            alphabet.SetText(newAlphabet);
        }

        public void SetDescription(string newDescription) {
            description.SetText(newDescription);
        }

        public void SetPicture(string langName, string newPicTooltip) {
            picture.LoadImage(langName, !alphabet.IsEmpty(), newPicTooltip);
        }
    }
}