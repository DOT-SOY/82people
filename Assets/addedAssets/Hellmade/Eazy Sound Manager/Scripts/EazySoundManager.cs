using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Hellmade.Sound
{
    /// <summary>
    /// ������� �Ҹ��� ����ϰ� �����ϴ� ������ �ϴ� ���� Ŭ�����Դϴ�.
    /// </summary>
    public class EazySoundManager : MonoBehaviour
    {
        /// <summary>
        /// ���� �Ŵ����� ������ ���ӿ�����Ʈ
        /// </summary>
        public static GameObject Gameobject { get { return Instance.gameObject; } }

        /// <summary>
        /// true�� �����ϸ� ������ ����� Ŭ���� ���� ���ο� ���� ������� ���õ˴ϴ�.
        /// </summary>
        public static bool IgnoreDuplicateMusic { get; set; }

        /// <summary>
        /// true�� �����ϸ� ������ ����� Ŭ���� ���� ���ο� ���� ������� ���õ˴ϴ�.
        /// </summary>
        public static bool IgnoreDuplicateSounds { get; set; }

        /// <summary>
        /// true�� �����ϸ� ������ ����� Ŭ���� ���� ���ο� UI ���� ������� ���õ˴ϴ�.
        /// </summary>
        public static bool IgnoreDuplicateUISounds { get; set; }

        /// <summary>
        /// �۷ι� ����
        /// </summary>
        public static float GlobalVolume { get; set; }

        /// <summary>
        /// �۷ι� ���� ����
        /// </summary>
        public static float GlobalMusicVolume { get; set; }

        /// <summary>
        /// �۷ι� ���� ����
        /// </summary>
        public static float GlobalSoundsVolume { get; set; }

        /// <summary>
        /// �۷ι� UI ���� ����
        /// </summary>
        public static float GlobalUISoundsVolume { get; set; }

        private static EazySoundManager instance = null;

        private static Dictionary<int, Audio> musicAudio;
        private static Dictionary<int, Audio> soundsAudio;
        private static Dictionary<int, Audio> UISoundsAudio;
        private static Dictionary<int, Audio> audioPool;

        private static bool initialized = false;

        private static EazySoundManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (EazySoundManager)FindObjectOfType(typeof(EazySoundManager));
                    if (instance == null)
                    {
                        // ���ӿ�����Ʈ ���� �� ������Ʈ �߰�
                        instance = (new GameObject("EazySoundManager")).AddComponent<EazySoundManager>();
                    }
                }
                return instance;
            }
        }

        static EazySoundManager()
        {
            Instance.Init();
        }

        /// <summary>
        /// ���� �Ŵ����� �ʱ�ȭ�մϴ�.
        /// </summary>
        private void Init()
        {
            if (!initialized)
            {
                musicAudio = new Dictionary<int, Audio>();
                soundsAudio = new Dictionary<int, Audio>();
                UISoundsAudio = new Dictionary<int, Audio>();
                audioPool = new Dictionary<int, Audio>();

                GlobalVolume = 1;
                GlobalMusicVolume = 1;
                GlobalSoundsVolume = 1;
                GlobalUISoundsVolume = 1;

                IgnoreDuplicateMusic = false;
                IgnoreDuplicateSounds = false;
                IgnoreDuplicateUISounds = false;

                initialized = true;
                DontDestroyOnLoad(this);
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// ���ο� ���� �ε�� �� �߻��ϴ� �̺�Ʈ
        /// </summary>
        /// <param name="scene">�ε�� ��</param>
        /// <param name="mode">�� �ε� ���</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // ���ӵ��� �ʴ� ��� ������� �����ϰ� ����
            RemoveNonPersistAudio(musicAudio);
            RemoveNonPersistAudio(soundsAudio);
            RemoveNonPersistAudio(UISoundsAudio);
        }

        private void Update()
        {
            UpdateAllAudio(musicAudio);
            UpdateAllAudio(soundsAudio);
            UpdateAllAudio(UISoundsAudio);
        }

        /// <summary>
        /// audioType�� ���� ����� ��ųʸ��� �����ɴϴ�.
        /// </summary>
        /// <param name="audioType">��ȯ�� ��ųʸ��� ����� Ÿ��</param>
        /// <returns>����� ��ųʸ�</returns>
        private static Dictionary<int, Audio> GetAudioTypeDictionary(Audio.AudioType audioType)
        {
            Dictionary<int, Audio> audioDict = new Dictionary<int, Audio>();
            switch (audioType)
            {
                case Audio.AudioType.Music:
                    audioDict = musicAudio;
                    break;
                case Audio.AudioType.Sound:
                    audioDict = soundsAudio;
                    break;
                case Audio.AudioType.UISound:
                    audioDict = UISoundsAudio;
                    break;
            }

            return audioDict;
        }

        /// <summary>
        /// ������ ����� Ÿ���� IgnoreDuplicates ������ �����ɴϴ�.
        /// </summary>
        /// <param name="audioType">��ȯ�� IgnoreDuplicates ������ ������ ��ġ�� ����� Ÿ��</param>
        /// <returns>IgnoreDuplicates ���� (bool)</returns>
        private static bool GetAudioTypeIgnoreDuplicateSetting(Audio.AudioType audioType)
        {
            switch (audioType)
            {
                case Audio.AudioType.Music:
                    return IgnoreDuplicateMusic;
                case Audio.AudioType.Sound:
                    return IgnoreDuplicateSounds;
                case Audio.AudioType.UISound:
                    return IgnoreDuplicateUISounds;
                default:
                    return false;
            }
        }

        /// <summary>
        /// ����� ��ųʸ��� ��� ����� ���¸� ������Ʈ�մϴ�.
        /// </summary>
        /// <param name="audioDict">������Ʈ�� ����� ��ųʸ�</param>
        private static void UpdateAllAudio(Dictionary<int, Audio> audioDict)
        {
            // ��� ������� ������Ʈ
            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                Audio audio = audioDict[key];
                audio.Update();

                // �� �̻� Ȱ��ȭ���� ����(��� ������ ����) ������� ����
                if (!audio.IsPlaying && !audio.Paused)
                {
                    Destroy(audio.AudioSource);

                    // ���߿� ������ �� �ֵ��� ����� Ǯ�� �߰�
                    audioPool.Add(key, audio);
                    audio.Pooled = true;
                    audioDict.Remove(key);
                }
            }
        }

        /// <summary>
        /// ����� ��ųʸ����� ���ӵ��� �ʴ� ��� ������� �����մϴ�.
        /// </summary>
        /// <param name="audioDict">���ӵ��� �ʴ� ������� ���ŵ� ����� ��ųʸ�</param>
        private static void RemoveNonPersistAudio(Dictionary<int, Audio> audioDict)
        {
            // ��� ������� Ȯ���ϰ� ���� ���� ���ӵ��� �ʴ� ��� ����
            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                Audio audio = audioDict[key];
                if (!audio.Persist && audio.Activated)
                {
                    Destroy(audio.AudioSource);
                    audioDict.Remove(key);
                }
            }

            // ����� Ǯ�� ��� ������� Ȯ���ϰ� ���� ���� ���ӵ��� �ʴ� ��� ����
            keys = new List<int>(audioPool.Keys);
            foreach (int key in keys)
            {
                Audio audio = audioPool[key];
                if (!audio.Persist && audio.Activated)
                {
                    audioPool.Remove(key);
                }
            }
        }

        /// <summary>
        /// Ǯ�� �ִ� ������� �����ϰ� �ش� ����� ��ųʸ��� �ٽ� �߰��մϴ�.
        /// </summary>
        /// <param name="audioType">������ ������� ����� Ÿ��</param>
        /// <param name="audioID">������ ������� ID</param>
        /// <returns>������� �����Ǹ� true, ������� ����� Ǯ�� ������ false</returns>
        public static bool RestoreAudioFromPool(Audio.AudioType audioType, int audioID)
        {
            if (audioPool.ContainsKey(audioID))
            {
                Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);
                audioDict.Add(audioID, audioPool[audioID]);
                audioPool.Remove(audioID);

                return true;
            }

            return false;
        }

        #region GetAudio Functions

        /// <summary>
        /// audioID�� ���� ������� ��ȯ�մϴ�. �ش� ������� ã�� �� ���� ��� null�� ��ȯ�մϴ�.
        /// </summary>
        /// <param name="audioID">�˻��� ������� ID</param>
        /// <returns>audioID�� ���� �����, �ش� ������� ã�� �� ���� ��� null</returns>
        public static Audio GetAudio(int audioID)
        {
            Audio audio;

            audio = GetMusicAudio(audioID);
            if (audio != null)
            {
                return audio;
            }

            audio = GetSoundAudio(audioID);
            if (audio != null)
            {
                return audio;
            }

            audio = GetUISoundAudio(audioID);
            if (audio != null)
            {
                return audio;
            }

            return null;
        }

        /// <summary>
        /// ������ audioClip�� ����ϴ� ù ��° ������� ��ȯ�մϴ�. �ش� ������� ã�� �� ���� ��� null�� ��ȯ�մϴ�.
        /// </summary>
        /// <param name="audioClip">�˻��� ����� Ŭ��</param>
        /// <returns>audioClip�� ����ϴ� ù ��° �����, �ش� ������� ã�� �� ���� ��� null</returns>
        public static Audio GetAudio(AudioClip audioClip)
        {
            Audio audio = GetMusicAudio(audioClip);
            if (audio != null)
            {
                return audio;
            }

            audio = GetSoundAudio(audioClip);
            if (audio != null)
            {
                return audio;
            }

            audio = GetUISoundAudio(audioClip);
            if (audio != null)
            {
                return audio;
            }

            return null;
        }

        /// <summary>
        /// audioID�� ���� ���� ������� ��ȯ�մϴ�. �ش� ������� ã�� �� ���� ��� null�� ��ȯ�մϴ�.
        /// </summary>
        /// <param name="audioID">��ȯ�� ���� ������� ID</param>
        /// <returns>audioID�� ���� ���� �����, �ش� ������� ã�� �� ���� ��� null</returns>
        public static Audio GetMusicAudio(int audioID)
        {
            return GetAudio(Audio.AudioType.Music, true, audioID);
        }

        /// <summary>
        /// ������ audioClip�� ����ϴ� ù ��° ���� ������� ��ȯ�մϴ�. �ش� ������� ã�� �� ���� ��� null�� ��ȯ�մϴ�.
        /// </summary>
        /// <param name="audioClip">�˻��� ���� ����� Ŭ��</param>
        /// <returns>audioClip�� ����ϴ� ù ��° ���� �����, �ش� ������� ã�� �� ���� ��� null</returns>
        public static Audio GetMusicAudio(AudioClip audioClip)
        {
            return GetAudio(Audio.AudioType.Music, true, audioClip);
        }

        /// <summary>
        /// audioID�� ���� ���� fx ������� ��ȯ�մϴ�. �ش� ������� ã�� �� ���� ��� null�� ��ȯ�մϴ�.
        /// </summary>
        /// <param name="audioID">��ȯ�� ���� fx ������� ID</param>
        /// <returns>audioID�� ���� ���� fx �����, �ش� ������� ã�� �� ���� ��� null</returns>
        public static Audio GetSoundAudio(int audioID)
        {
            return GetAudio(Audio.AudioType.Sound, true, audioID);
        }

        /// <summary>
        /// ������ audioClip�� ����ϴ� ù ��° ���� ������� ��ȯ�մϴ�. �ش� ������� ã�� �� ���� ��� null�� ��ȯ�մϴ�.
        /// </summary>
        /// <param name="audioClip">�˻��� ���� ����� Ŭ��</param>
        /// <returns>audioClip�� ����ϴ� ù ��° ���� �����, �ش� ������� ã�� �� ���� ��� null</returns>
        public static Audio GetSoundAudio(AudioClip audioClip)
        {
            return GetAudio(Audio.AudioType.Sound, true, audioClip);
        }

        /// <summary>
        /// audioID�� ���� UI ���� fx ������� ��ȯ�մϴ�. �ش� ������� ã�� �� ���� ��� null�� ��ȯ�մϴ�.
        /// </summary>
        /// <param name="audioID">��ȯ�� UI ���� fx ������� ID</param>
        /// <returns>audioID�� ���� UI ���� fx �����, �ش� ������� ã�� �� ���� ��� null</returns>
        public static Audio GetUISoundAudio(int audioID)
        {
            return GetAudio(Audio.AudioType.UISound, true, audioID);
        }

        /// <summary>
        /// ������ audioClip�� ����ϴ� ù ��° UI ���� ������� ��ȯ�մϴ�. �ش� ������� ã�� �� ���� ��� null�� ��ȯ�մϴ�.
        /// </summary>
        /// <param name="audioClip">�˻��� UI ���� ����� Ŭ��</param>
        /// <returns>audioClip�� ����ϴ� ù ��° UI ���� �����, �ش� ������� ã�� �� ���� ��� null</returns>
        public static Audio GetUISoundAudio(AudioClip audioClip)
        {
            return GetAudio(Audio.AudioType.UISound, true, audioClip);
        }

        private static Audio GetAudio(Audio.AudioType audioType, bool usePool, int audioID)
        {
            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);

            if (audioDict.ContainsKey(audioID))
            {
                return audioDict[audioID];
            }

            if (usePool && audioPool.ContainsKey(audioID) && audioPool[audioID].Type == audioType)
            {
                return audioPool[audioID];
            }

            return null;
        }

        private static Audio GetAudio(Audio.AudioType audioType, bool usePool, AudioClip audioClip)
        {
            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);

            List<int> audioTypeKeys = new List<int>(audioDict.Keys);
            List<int> poolKeys = new List<int>(audioPool.Keys);
            List<int> keys = usePool ? audioTypeKeys.Concat(poolKeys).ToList() : audioTypeKeys;
            foreach (int key in keys)
            {
                Audio audio = null;
                if (audioDict.ContainsKey(key))
                {
                    audio = audioDict[key];
                }
                else if (audioPool.ContainsKey(key))
                {
                    audio = audioPool[key];
                }
                if (audio == null)
                {
                    return null;
                }
                if (audio.Clip == audioClip && audio.Type == audioType)
                {
                    return audio;
                }
            }

            return null;
        }

        #endregion

        #region Prepare Function

        /// <summary>
        /// ��� ������ �غ��ϰ� �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="clip">�غ��� ����� Ŭ��</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PrepareMusic(AudioClip clip)
        {
            return PrepareAudio(Audio.AudioType.Music, clip, 1f, false, false, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// ��� ������ �غ��ϰ� �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="clip">�غ��� ����� Ŭ��</param>
        /// <param name="volume">������ ����</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PrepareMusic(AudioClip clip, float volume)
        {
            return PrepareAudio(Audio.AudioType.Music, clip, volume, false, false, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// ��� ������ �غ��ϰ� �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="clip">�غ��� ����� Ŭ��</param>
        /// <param name="volume">������ ����</param>
        /// <param name="loop">������ �ݺ��Ǵ��� ����</param>
        /// <param name="persist">������� �� ���� ���� ���ӵǴ��� ����</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PrepareMusic(AudioClip clip, float volume, bool loop, bool persist)
        {
            return PrepareAudio(Audio.AudioType.Music, clip, volume, loop, persist, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// ��� ������ �غ��ϰ� �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="clip">�غ��� ����� Ŭ��</param>
        /// <param name="volume">������ ����</param>
        /// <param name="loop">������ �ݺ��Ǵ��� ����</param>
        /// <param name="persist">������� �� ���� ���� ���ӵǴ��� ����</param>
        /// <param name="fadeInSeconds">������� ���̵� ��(������ ������)�Ǵ� �� �ɸ��� �ð�(��)</param>
        /// <param name="fadeOutSeconds">������� ���̵� �ƿ�(������ ������)�Ǵ� �� �ɸ��� �ð�(��)</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PrepareMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds)
        {
            return PrepareAudio(Audio.AudioType.Music, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, -1f, null);
        }

        /// <summary>
        /// ��� ������ �غ��ϰ� �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="clip">�غ��� ����� Ŭ��</param>
        /// <param name="volume">������ ����</param>
        /// <param name="loop">������ �ݺ��Ǵ��� ����</param>
        /// <param name="persist">������� �� ���� ���� ���ӵǴ��� ����</param>
        /// <param name="fadeInSeconds">������� ���̵� ��(������ ������)�Ǵ� �� �ɸ��� �ð�(��)</param>
        /// <param name="fadeOutSeconds">������� ���̵� �ƿ�(������ ������)�Ǵ� �� �ɸ��� �ð�(��)</param>
        /// <param name="currentMusicfadeOutSeconds">���� ���� ������� ���̵� �ƿ��Ǵ� �� �ɸ��� �ð�(��). ���� ������ �ڽ��� ���̵� �ƿ� �ð��� �����մϴ�. -1�� ���޵Ǹ� ���� ������ �ڽ��� ���̵� �ƿ� �ð��� �����մϴ�.</param>
        /// <param name="sourceTransform">������ �ҽ��� �Ǵ� Ʈ������(3D ������� �˴ϴ�). 3D ������� �ʿ����� ���� ��� null�� ����ϼ���.</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PrepareMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
        {
            return PrepareAudio(Audio.AudioType.Music, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, currentMusicfadeOutSeconds, sourceTransform);
        }

        /// <summary>
        /// ���� fx�� �غ��ϰ� �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="clip">�غ��� ����� Ŭ��</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PrepareSound(AudioClip clip)
        {
            return PrepareAudio(Audio.AudioType.Sound, clip, 1f, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// ���� fx�� �غ��ϰ� �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="clip">�غ��� ����� Ŭ��</param>
        /// <param name="volume">������ ����</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PrepareSound(AudioClip clip, float volume)
        {
            return PrepareAudio(Audio.AudioType.Sound, clip, volume, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// ���� fx�� �غ��ϰ� �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="clip">�غ��� ����� Ŭ��</param>
        /// <param name="loop">���尡 �ݺ��Ǵ��� ����</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PrepareSound(AudioClip clip, bool loop)
        {
            return PrepareAudio(Audio.AudioType.Sound, clip, 1f, loop, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// ���� fx�� �غ��ϰ� �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="clip">�غ��� ����� Ŭ��</param>
        /// <param name="volume">������ ����</param>
        /// <param name="loop">���尡 �ݺ��Ǵ��� ����</param>
        /// <param name="sourceTransform">������ �ҽ��� �Ǵ� Ʈ������(3D ������� �˴ϴ�). 3D ������� �ʿ����� ���� ��� null�� ����ϼ���.</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PrepareSound(AudioClip clip, float volume, bool loop, Transform sourceTransform)
        {
            return PrepareAudio(Audio.AudioType.Sound, clip, volume, loop, false, 0f, 0f, -1f, sourceTransform);
        }

        /// <summary>
        /// UI ���� fx�� �غ��ϰ� �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="clip">�غ��� ����� Ŭ��</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PrepareUISound(AudioClip clip)
        {
            return PrepareAudio(Audio.AudioType.UISound, clip, 1f, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// UI ���� fx�� �غ��ϰ� �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="clip">�غ��� ����� Ŭ��</param>
        /// <param name="volume">������ ����</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PrepareUISound(AudioClip clip, float volume)
        {
            return PrepareAudio(Audio.AudioType.UISound, clip, volume, false, false, 0f, 0f, -1f, null);
        }

        private static int PrepareAudio(Audio.AudioType audioType, AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
        {
            if (clip == null)
            {
                Debug.LogError("[Eazy Sound Manager] ����� Ŭ���� null�Դϴ�.", clip);
            }

            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);
            bool ignoreDuplicateAudio = GetAudioTypeIgnoreDuplicateSetting(audioType);

            if (ignoreDuplicateAudio)
            {
                Audio duplicateAudio = GetAudio(audioType, true, clip);
                if (duplicateAudio != null)
                {
                    return duplicateAudio.AudioID;
                }
            }

            // audioSource ����
            Audio audio = new Audio(audioType, clip, loop, persist, volume, fadeInSeconds, fadeOutSeconds, sourceTransform);

            // ��ųʸ��� �߰�
            audioDict.Add(audio.AudioID, audio);

            return audio.AudioID;
        }

        #endregion

        #region Play Functions

        /// <summary>
        /// ��� ������ ����մϴ�.
        /// </summary>
        /// <param name="clip">����� ����� Ŭ��</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PlayMusic(AudioClip clip)
        {
            return PlayAudio(Audio.AudioType.Music, clip, 1f, false, false, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// ��� ������ ����մϴ�.
        /// </summary>
        /// <param name="clip">����� ����� Ŭ��</param>
        /// <param name="volume">������ ����</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PlayMusic(AudioClip clip, float volume)
        {
            return PlayAudio(Audio.AudioType.Music, clip, volume, false, false, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// ��� ������ ����մϴ�.
        /// </summary>
        /// <param name="clip">����� ����� Ŭ��</param>
        /// <param name="volume">������ ����</param>
        /// <param name="loop">������ �ݺ��Ǵ��� ����</param>
        /// <param name="persist">������� �� ���� ���� ���ӵǴ��� ����</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PlayMusic(AudioClip clip, float volume, bool loop, bool persist)
        {
            return PlayAudio(Audio.AudioType.Music, clip, volume, loop, persist, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// ��� ������ ����մϴ�.
        /// </summary>
        /// <param name="clip">����� ����� Ŭ��</param>
        /// <param name="volume">������ ����</param>
        /// <param name="loop">������ �ݺ��Ǵ��� ����</param>
        /// <param name="persist">������� �� ���� ���� ���ӵǴ��� ����</param>
        /// <param name="fadeInSeconds">������� ���̵� ��(������ ������)�Ǵ� �� �ɸ��� �ð�(��)</param>
        /// <param name="fadeOutSeconds">������� ���̵� �ƿ�(������ ������)�Ǵ� �� �ɸ��� �ð�(��)</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PlayMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds)
        {
            return PlayAudio(Audio.AudioType.Music, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, -1f, null);
        }

        /// <summary>
        /// ��� ������ ����մϴ�.
        /// </summary>
        /// <param name="clip">����� ����� Ŭ��</param>
        /// <param name="volume">������ ����</param>
        /// <param name="loop">������ �ݺ��Ǵ��� ����</param>
        /// <param name="persist">������� �� ���� ���� ���ӵǴ��� ����</param>
        /// <param name="fadeInSeconds">������� ���̵� ��(������ ������)�Ǵ� �� �ɸ��� �ð�(��)</param>
        /// <param name="fadeOutSeconds">������� ���̵� �ƿ�(������ ������)�Ǵ� �� �ɸ��� �ð�(��)</param>
        /// <param name="currentMusicfadeOutSeconds">���� ���� ������� ���̵� �ƿ��Ǵ� �� �ɸ��� �ð�(��). ���� ������ �ڽ��� ���̵� �ƿ� �ð��� �����մϴ�. -1�� ���޵Ǹ� ���� ������ �ڽ��� ���̵� �ƿ� �ð��� �����մϴ�.</param>
        /// <param name="sourceTransform">������ �ҽ��� �Ǵ� Ʈ������(3D ������� �˴ϴ�). 3D ������� �ʿ����� ���� ��� null�� ����ϼ���.</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PlayMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
        {
            return PlayAudio(Audio.AudioType.Music, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, currentMusicfadeOutSeconds, sourceTransform);
        }

        /// <summary>
        /// ���� fx�� ����մϴ�.
        /// </summary>
        /// <param name="clip">����� ����� Ŭ��</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PlaySound(AudioClip clip)
        {
            return PlayAudio(Audio.AudioType.Sound, clip, 1f, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// ���� fx�� ����մϴ�.
        /// </summary>
        /// <param name="clip">����� ����� Ŭ��</param>
        /// <param name="volume">������ ����</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PlaySound(AudioClip clip, float volume)
        {
            return PlayAudio(Audio.AudioType.Sound, clip, volume, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// ���� fx�� ����մϴ�.
        /// </summary>
        /// <param name="clip">����� ����� Ŭ��</param>
        /// <param name="loop">���尡 �ݺ��Ǵ��� ����</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PlaySound(AudioClip clip, bool loop)
        {
            return PlayAudio(Audio.AudioType.Sound, clip, 1f, loop, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// ���� fx�� ����մϴ�.
        /// </summary>
        /// <param name="clip">����� ����� Ŭ��</param>
        /// <param name="volume">������ ����</param>
        /// <param name="loop">���尡 �ݺ��Ǵ��� ����</param>
        /// <param name="sourceTransform">������ �ҽ��� �Ǵ� Ʈ������(3D ������� �˴ϴ�). 3D ������� �ʿ����� ���� ��� null�� ����ϼ���.</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PlaySound(AudioClip clip, float volume, bool loop, Transform sourceTransform)
        {
            return PlayAudio(Audio.AudioType.Sound, clip, volume, loop, false, 0f, 0f, -1f, sourceTransform);
        }

        /// <summary>
        /// UI ���� fx�� ����մϴ�.
        /// </summary>
        /// <param name="clip">����� ����� Ŭ��</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PlayUISound(AudioClip clip)
        {
            return PlayAudio(Audio.AudioType.UISound, clip, 1f, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// UI ���� fx�� ����մϴ�.
        /// </summary>
        /// <param name="clip">����� ����� Ŭ��</param>
        /// <param name="volume">������ ����</param>
        /// <returns>������ ����� ��ü�� ID</returns>
        public static int PlayUISound(AudioClip clip, float volume)
        {
            return PlayAudio(Audio.AudioType.UISound, clip, volume, false, false, 0f, 0f, -1f, null);
        }

        private static int PlayAudio(Audio.AudioType audioType, AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
        {
            int audioID = PrepareAudio(audioType, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, currentMusicfadeOutSeconds, sourceTransform);

            // ���� ��� ���� ��� ���� ����
            if (audioType == Audio.AudioType.Music)
            {
                StopAllMusic(currentMusicfadeOutSeconds);
            }

            GetAudio(audioType, false, audioID).Play();

            return audioID;
        }

        #endregion

        #region Stop Functions

        /// <summary>
        /// ��� ����� ��� ����
        /// </summary>
        public static void StopAll()
        {
            StopAll(-1f);
        }

        /// <summary>
        /// ��� ����� ��� ����
        /// </summary>
        /// <param name="musicFadeOutSeconds">��� ���� ������� ���̵� �ƿ��Ǵ� �� �ɸ��� �ð�(��). �ڽ��� ���̵� �ƿ� �ð��� �����մϴ�. -1�� ���޵Ǹ� ��� ������ �ڽ��� ���̵� �ƿ� �ð��� �����մϴ�.</param>
        public static void StopAll(float musicFadeOutSeconds)
        {
            StopAllMusic(musicFadeOutSeconds);
            StopAllSounds();
            StopAllUISounds();
        }

        /// <summary>
        /// ��� ���� ��� ����
        /// </summary>
        public static void StopAllMusic()
        {
            StopAllAudio(Audio.AudioType.Music, -1f);
        }

        /// <summary>
        /// ��� ���� ��� ����
        /// </summary>
        /// <param name="fadeOutSeconds">��� ���� ������� ���̵� �ƿ��Ǵ� �� �ɸ��� �ð�(��). �ڽ��� ���̵� �ƿ� �ð��� �����մϴ�. -1�� ���޵Ǹ� ��� ������ �ڽ��� ���̵� �ƿ� �ð��� �����մϴ�.</param>
        public static void StopAllMusic(float fadeOutSeconds)
        {
            StopAllAudio(Audio.AudioType.Music, fadeOutSeconds);
        }

        /// <summary>
        /// ��� ���� fx ��� ����
        /// </summary>
        public static void StopAllSounds()
        {
            StopAllAudio(Audio.AudioType.Sound, -1f);
        }

        /// <summary>
        /// ��� UI ���� fx ��� ����
        /// </summary>
        public static void StopAllUISounds()
        {
            StopAllAudio(Audio.AudioType.UISound, -1f);
        }

        private static void StopAllAudio(Audio.AudioType audioType, float fadeOutSeconds)
        {
            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);

            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                Audio audio = audioDict[key];
                if (fadeOutSeconds > 0)
                {
                    audio.FadeOutSeconds = fadeOutSeconds;
                }
                audio.Stop();
            }
        }

        #endregion

        #region Pause Functions

        /// <summary>
        /// ��� ����� �Ͻ� ����
        /// </summary>
        public static void PauseAll()
        {
            PauseAllMusic();
            PauseAllSounds();
            PauseAllUISounds();
        }

        /// <summary>
        /// ��� ���� �Ͻ� ����
        /// </summary>
        public static void PauseAllMusic()
        {
            PauseAllAudio(Audio.AudioType.Music);
        }

        /// <summary>
        /// ��� ���� fx �Ͻ� ����
        /// </summary>
        public static void PauseAllSounds()
        {
            PauseAllAudio(Audio.AudioType.Sound);
        }

        /// <summary>
        /// ��� UI ���� fx �Ͻ� ����
        /// </summary>
        public static void PauseAllUISounds()
        {
            PauseAllAudio(Audio.AudioType.UISound);
        }

        private static void PauseAllAudio(Audio.AudioType audioType)
        {
            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);

            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                Audio audio = audioDict[key];
                audio.Pause();
            }
        }

        #endregion

        #region Resume Functions

        /// <summary>
        /// ��� ����� �簳
        /// </summary>
        public static void ResumeAll()
        {
            ResumeAllMusic();
            ResumeAllSounds();
            ResumeAllUISounds();
        }

        /// <summary>
        /// ��� ���� �簳
        /// </summary>
        public static void ResumeAllMusic()
        {
            ResumeAllAudio(Audio.AudioType.Music);
        }

        /// <summary>
        /// ��� ���� fx �簳
        /// </summary>
        public static void ResumeAllSounds()
        {
            ResumeAllAudio(Audio.AudioType.Sound);
        }

        /// <summary>
        /// ��� UI ���� fx �簳
        /// </summary>
        public static void ResumeAllUISounds()
        {
            ResumeAllAudio(Audio.AudioType.UISound);
        }

        private static void ResumeAllAudio(Audio.AudioType audioType)
        {
            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);

            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                Audio audio = audioDict[key];
                audio.Resume();
            }
        }

        #endregion
    }
}
