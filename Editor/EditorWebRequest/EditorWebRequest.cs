
namespace TeaSpoons.PackageCore.Editor
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking;

    /// <summary>
    /// A wrapper for <see cref="UnityWebRequest"/>s that can wait in the editor and triggers a callback if the request was successful.
    /// </summary>
    public class EditorWebRequest
    {
        private UnityWebRequest request;
        private Action<DownloadHandler> successCallback;

        public bool IsDone { get; private set; } = false;

        /// <summary>
        /// Creates a new <see cref="EditorWebRequest"/>.
        /// </summary>
        /// <param name="request">A <see cref="UnityWebRequest"/> that has <b>not</b> been sent yet.</param>
        /// <param name="successCallback">The callback to trigger if the <paramref name="request"/> was successful.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
        public EditorWebRequest(UnityWebRequest request, Action<DownloadHandler> successCallback = null)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            this.request = request;
            this.successCallback = successCallback;
            
            request.SendWebRequest();

            EditorApplication.update += OnUpdate;
        }

#line hidden
        public void Cancel()
        {
            StopUpdating();
        }

        private void OnUpdate()
        {
            if (request.isDone)
            {
                StopUpdating();
                Finish();
            }
        }

        private void StopUpdating()
        {
            EditorApplication.update -= OnUpdate;
            IsDone = true;
        }

        private void Finish()
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                successCallback?.Invoke(request.downloadHandler);
            }
            else
            {
                Debug.LogError($"<b>EditorWebRequest failed</b>.\n[HTTP/{request.responseCode}] {request.uri}\n");
            }
        }
#line default
    }
}
