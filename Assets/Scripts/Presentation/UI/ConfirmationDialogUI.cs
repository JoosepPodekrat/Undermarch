﻿using System;
using TMPro;
using Undermarch.Presentation.Sounds;
using UnityEngine;
using UnityEngine.UI;

namespace Undermarch.Presentation.UI
{
    /// <summary>
    /// A reusable confirmation dialog component.
    /// Shows a message with Yes/No buttons.
    /// </summary>
    public class ConfirmationDialogUI : MonoBehaviour
    {
        public GameObject dialogPanel;
        public TextMeshProUGUI messageText;
        public Button yesButton;
        public Button noButton;

        private Action onConfirmCallback;
        private Action onCancelCallback;
        private bool isInitialized;

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes button listeners. Can be called after buttons are assigned.
        /// </summary>
        public void Initialize()
        {
            if (isInitialized) return;

            if (yesButton != null)
            {
                yesButton.onClick.AddListener(OnYesClicked);
            }

            if (noButton != null)
            {
                noButton.onClick.AddListener(OnNoClicked);
            }

            // Start hidden
            if (dialogPanel != null)
            {
                dialogPanel.SetActive(false);
            }

            // Mark as initialized only if we actually had buttons to hook up
            if (yesButton != null && noButton != null)
            {
                isInitialized = true;
            }
        }

        /// <summary>
        /// Shows the confirmation dialog with the specified message.
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="onConfirm">Action to invoke when Yes is clicked</param>
        /// <param name="onCancel">Optional action to invoke when No is clicked</param>
        public void Show(string message, Action onConfirm, Action onCancel = null)
        {
            if (dialogPanel == null || messageText == null)
            {
                Debug.LogError("ConfirmationDialogUI: Missing panel or text references!");
                return;
            }

            messageText.text = message;
            onConfirmCallback = onConfirm;
            onCancelCallback = onCancel;
            dialogPanel.SetActive(true);
        }

        /// <summary>
        /// Hides the confirmation dialog.
        /// </summary>
        public void Hide()
        {
            if (dialogPanel != null)
            {
                dialogPanel.SetActive(false);
            }
            onConfirmCallback = null;
            onCancelCallback = null;
        }

        private void OnYesClicked()
        {
            UIAudioManager.Instance?.PlayButtonClick();
            var callback = onConfirmCallback;
            Hide();
            callback?.Invoke();
        }

        private void OnNoClicked()
        {
            UIAudioManager.Instance?.PlayButtonClick();
            var callback = onCancelCallback;
            Hide();
            callback?.Invoke();
        }
    }
}
