import { createStore } from "vuex";
import axios from "axios";
import { useToast } from "vue-toastification";
import router from "@/router/router";

const toast = useToast();

const store = createStore({
    state: {
        captcha: null,
        serverUrl: import.meta.env.VITE_API_SERVER_URL,
        environment: import.meta.env.VITE_API_ENVIRONMENT,
        roles: [],
        nickname: null,
        chapter: null,
        chapters: [],
        chapterNodes: [],
        theme: null,
        themes: [],
        documents: [],
        documentNodes: [],
        folderPaths: [],
        messages: [],
        message: null,
        unreadMessagesCount: 0,
        searchResult: [],
        showSearchBar: true,
        title: null,
        imageNames: [],
        showChapterList: true,
        showRightColumn: false,
        pending: false,
        users: [],
        user: null,
        currentUser: null,
        sessionStorageKeys: {
          chaptersKey: 'chapters',
          chapterNodesKey: 'chapterNodes',
          documentsKey: 'documents',
          documentNodesKey: 'documentNodes',
          currentUserKey: 'currentUser',
          rolesKey: 'roles'
        }
    },
    getters: {
      // CHAPTERS
        getChapter(state) {
          return state.chapter;
        },
        getChapters(state) {
          return state.chapters;
        },
        getChapterNodes(state) {
          return state.chapterNodes;
        },
        getShowChapterList(state) {
            return state.showChapterList;
        },
        getImageNames(state) {
            return state.imageNames;
        },

        // USERS
        getCurrentUser(state) {
          return state.currentUser;
        },
        getUsers(state) {
            return state.users;
        },
        getUser(state) {
            return state.user;
        },
        getRoles(state) {
            return state.roles;
        },
        getNickname(state) {
            return state.nickname;
        },
        isAdmin(state) {
            return state.roles.includes('Admin');
        },
        isOwner(state) {
            return state.roles.includes('Owner');
        },
        isUser(state) {
            return state.roles.includes('User');
        },
        isAuthenticated(state) {
            return state.nickname && state.roles && state.roles.length > 0;
        },

        // THEMES
        getTheme(state) {
            return state.theme;
        },
        getThemes(state) {
            return state.themes;
        },

        // DOCUMENTS
        getDocuments(state) {
          return state.documents;
        },
        getDocumentNodes(state) {
          return state.documentNodes;
        },
        getFolderPaths(state) {
            state.folderPaths = ['...'];
            state.documentNodes.forEach(node => getPaths(node.children));

            function getPaths(nodes) {
                nodes.forEach(node => {
                    if(node.data.type === 'folder') {
                        state.folderPaths.push(node.data.path.split('\\').slice(1).join('\\'));
                        getPaths(node.children);
                    }
                });
            };

            return state.folderPaths;
        },

        // MESSAGES
        getMessages(state) {
            return state.messages;
        },
        getMessage(state) {
            return state.message;
        },
        getUnreadMessagesCount(state) {
            return state.unreadMessagesCount;
        },

        // SEARCH
        getSearchResult(state) {
            return state.searchResult;
        },

        // ENVIRONMENT
        serverUrl(state) {
            return state.serverUrl;
        },
        environment(state) {
            return state.environment;
        },
        getPending(state) {
            return state.pending;
        },
        getShowRightColumn(state) {
            return state.showRightColumn;
        },
        getTitle(state) {
            return state.title;
        },
        getCaptcha(state) {
            return state.captcha;
        }
    },
    mutations: {
      // USERS
        setRoles(state, roles) {
            state.roles = roles;
            sessionStorage.setItem(state.sessionStorageKeys.rolesKey, JSON.stringify(roles));
        },
        setNickname(state, userNickname) {
            state.nickname = userNickname;
        },
        setCurrentUser(state, currentUser) {
          state.currentUser = currentUser;
          sessionStorage.setItem(state.sessionStorageKeys.currentUserKey, JSON.stringify(currentUser));
        },
        setUsers(state, value) {
            state.users = value;
        },
        setUser(state, value) {
            state.user = value;
        },

        // SEARCH
        renderSearchBar(state) {
            state.title = null;
            state.showSearchBar = true;
        },
        setTitle(state, value) {
            state.title = value;
            state.showSearchBar = false;
        },
        setSearchResult(state, searchResult) {
            state.searchResult = searchResult;
        },

        // CHAPTERS
        setChapter(state, chapter) {
            state.chapter = chapter;
        },
        setChapters(state, chapters) {
          state.chapters = chapters;
          sessionStorage.setItem(state.sessionStorageKeys.chaptersKey, JSON.stringify(chapters));
        },
        setChapterNodes(state, chapterNodes) {
          state.chapterNodes = chapterNodes;
          sessionStorage.setItem(state.sessionStorageKeys.chapterNodesKey, JSON.stringify(chapterNodes));
        },
        setShowChapterList(state, value) {
            state.showChapterList = value;
        },

        // THEMES
        setTheme(state, theme) {
            state.theme = theme;
        },
        setThemes(state, themes) {
            state.themes = themes;
        },

        // DOCUMENTS
        setDocuments(state, documents) {
          sessionStorage.setItem(state.sessionStorageKeys.documentsKey, JSON.stringify(documents));
          state.documents = documents;
        },
        setDocumentNodes(state, documentNodes) {
          sessionStorage.setItem(state.sessionStorageKeys.documentNodesKey, JSON.stringify(documentNodes));
          state.documentNodes = documentNodes;
        },

        // MESSAGES
        setMessages(state, messages) {
            state.messages = messages;
        },
        setMessage(state, message) {
            state.message = message;
        },
        setUnreadMessagesCount(state, count) {
            state.unreadMessagesCount = count;
        },
        setMessageById(state, messageId) {
            state.message = state.messages.find(x => x.messageId === messageId);
        },

        // ENVIRONMENT
        setShowRightColumn(state, value) {
            state.showRightColumn = value;
        },
        setCaptcha(state, value) {
            state.captcha = value;
        },
        setPending(state, value) {
            state.pending = value;
        },
        setImageNames(state, value) {
            state.imageNames = value;
        }
    },
    actions: {
      // CHAPTERS
        async downloadChapter({commit, state}, chapterId ) {
            const response = await axios.get(`${state.serverUrl}/chapters/get/${chapterId}`);

            try {
              if(response.status === 200) {
                const chapter = response.data;
                commit('setChapter', chapter);
                commit('setThemes', chapter.themes);
              }
            }
            catch(error) {
              toast.error(error.response.data.errorText);
            }
        },
        async downloadChapters({dispatch, commit, state}) {
          const storedChapters = sessionStorage.getItem(state.sessionStorageKeys.chaptersKey);

          if(!storedChapters) {
            const response = await axios.get(`${state.serverUrl}/chapters/getlist`);

            try {
              if(response.status === 200) {
                const chapters = response.data;
                await dispatch('downloadChapterNodes');
                commit('setChapters', chapters);
              }
            }
            catch(error) {
              toast.error(error.response.data.errorText);
            }
          }
          else {
            commit('setChapters', JSON.parse(storedChapters));
          }
        },
        async downloadChapterNodes({commit, state}) {
          const storedChapterNodes = sessionStorage.getItem(state.sessionStorageKeys.chapterNodesKey);

          if(!storedChapterNodes) {
            const chapterNodes = (await axios.get(`${state.serverUrl}/chapters/getnodes`)).data;
            commit('setChapterNodes', chapterNodes);
          }
          else {
            commit('setChapterNodes', JSON.parse(storedChapterNodes));
          }
        },
        async createChapter({dispatch, state}, formData) {
          await axios.post(`${store.getters.serverUrl}/chapters/create`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
                'Accept': ''
            }})
            .then(async response => {
              const status = response.status;

              if(status === 200) {
                  toast.success('Раздел создан');
                  sessionStorage.removeItem(state.sessionStorageKeys.chaptersKey)
                  sessionStorage.removeItem(state.sessionStorageKeys.chapterNodesKey)
                  await dispatch('downloadChapters');
                  await dispatch('downloadChapterNodes');
                  router.push(`/chapters/${response.data.chapterId}`);
              }
            })
            .catch(error => {
              if(error.response) {
                  toast.error(error.response.data.errorText)
              }}
            );
        },
        async deleteChapter({dispatch, state}, chapter) {
          if(!window.confirm('Этот раздел и его темы будут удалены. Вы уверены?')) {
              return;
          }

          const url = `${state.serverUrl}/chapters/delete/` + chapter.chapterId;
          const response = await axios.delete(url, null);

          try {
            const status = response.status;
              if(status === 200) {
                toast.success('Раздел успешно удален');
                sessionStorage.removeItem(state.sessionStorageKeys.chaptersKey)
                sessionStorage.removeItem(state.sessionStorageKeys.chapterNodesKey)
                await dispatch('downloadChapters');
                router.push(`/`);
              }
          }
          catch(error) {
            if(error.response) {
              toast.error(error.response.data.errorText)
            }
          }
        },
        async updateChapter({dispatch, state}, chapter) {
          const response = await axios.put(`${state.serverUrl}/chapters/update`,  chapter, {
              headers: {
                  'Content': 'application/json',
                  'Accept': '*/*'
              }
          });

          try {
            if(response.status === 200) {
                  toast.success('Раздел успешно обновлен');
                  sessionStorage.removeItem(state.sessionStorageKeys.chaptersKey);
                  sessionStorage.removeItem(state.sessionStorageKeys.chapterNodesKey);
                  await dispatch('downloadChapter',  chapter.chapterId);
                  await dispatch('downloadChapters');
                  router.push(`/chapters/${chapter.chapterId}${chapter.themes.length > 0 ? '/' + chapter.themes[0].themeId : '' }`);
              }
          }
          catch(error) {
            if(error.response) {
              toast.error(error.response.data.errorText)
            }
          }
        },

        // THEMES
        async downloadTheme({commit, state}, themeId ) {
            let url = `${state.serverUrl}/themes/get/`;

            if (themeId) {
                url += `${themeId}`;
            }
            else if (state.chapter.themes.length > 0){
                url += `${state.chapter.themes[0].themeId}`;
            }
            else {
                return;
            }

            commit('setPending', true);

            try {
                const theme = (await axios.get(url)
                .then(response => response.data)
                .catch(error => {
                    if(error.response) {
                        toast.error(error.response.data.errorText)
                    }
                }));

                commit('setTheme', theme);
            }
            finally {
                commit('setPending', false);
            }
        },

        // DOCUMENTS
        async downloadDocuments({commit, state}) {
          const storedDocuments = sessionStorage.getItem(state.sessionStorageKeys.documentsKey);

          if(!storedDocuments) {
            const documents = (await axios.get(`${state.serverUrl}/documents/getlist`)
                .then(response => response.data)
                .catch(error => {
                    if(error.response) {
                        toast.error(error.response.data.errorText)
                    }
                }));

            commit('setDocuments', documents);
          }
          else {
            commit('setDocuments', JSON.parse(storedDocuments));
          }
        },
        async downloadDocumentNodes({commit, state}) {
          const storedDocumentNodes = sessionStorage.getItem(state.sessionStorageKeys.documentNodesKey);

          if(!storedDocumentNodes) {
            const documentNodes = (await axios.get(`${state.serverUrl}/documents/getnodes`)
                .then(response => response.data)
                .catch(error => {
                    if(error.response) {
                        toast.error(error.response.data.errorText)
                    }
                }));

            commit('setDocumentNodes', documentNodes);
          }
          else {
            commit('setDocumentNodes', JSON.parse(storedDocumentNodes));
          }
        },

        // MESSAGES
        async downloadMessages({commit, state}, isRead) {
            const messages = (await axios.get(`${state.serverUrl}/feedback/getmessages/${isRead}`)
                .then(response => response.data)
                .catch(error => {
                    if(error.response) {
                        toast.error(error.response.data.errorText)
                    }
                }));

            commit('setMessages', messages);
        },
        async downloadMessage({commit, state}, messageId) {
            const message = await axios.get(`${state.serverUrl}/feedback/getmessage/${messageId}`)
                .then(response => {
                    if(response.status === 200) {
                        return response.data;
                    }
                })
                .catch(error => {
                    if(error.response) {
                        toast.error(error.response.data.errorText)
                    }
                });

            commit('setMessage', message);
        },
        async downloadUnreadMessagesCount({commit, state}) {
            const count = await axios.get(`${state.serverUrl}/feedback/getunreadmessagescount`)
                .then(response => {
                    if(response.status === 200) {
                        return response.data;
                    }
                });

            commit('setUnreadMessagesCount', count);
        },

        // SEARCH
        async downloadSearchResult({commit, state}, searchLine) {
            const searchResult = (await axios.post(`${state.serverUrl}/search/getsearchresult`, {
                 searchLine: searchLine
              }
            )
            .then(response => response.data)
            .catch(error => {
                if(error.response) {
                    toast.error(error.response.data.errorText)
                }
            }));

            commit('setSearchResult', searchResult);
        },

        // CAPTCHA
        async downloadCaptcha({commit, state}) {
            const captcha = (await axios.get(`${state.serverUrl}/captcha/get`)
                .then(response => response.data)
                .catch(error => {
                    if(error.response) {
                        toast.error(error.response.data.errorText);
                    }
                }
            )
        );

            commit('setCaptcha', captcha);
        },

        // IMAGES
        async downloadImageNames({commit, state}) {
            const imageNames = (await axios.get(`${state.serverUrl}/home/getimagenames`)
                .then(response => response.data)
                .catch(error => {
                    if(error.response) {
                        toast.error(error.response.data.errorText);
                    }
                }
            ));

            commit('setImageNames', imageNames);
        },

        // USERS
        async downloadUsers({commit, state}) {
            const users = (await axios.get(`${state.serverUrl}/administration/getusers`)
                .then(response => response.data)
                .catch(error => {
                    if(error.response) {
                        toast.error(error.response.data.errorText);
                    }
                }
            ));

            commit('setUsers', users);
        },
        async downloadUser({commit, state}, userId) {
            const user = (await axios.get(`${state.serverUrl}/administration/getuser/${userId}`)
                .then(response => response.data)
                .catch(error => {
                    if(error.response) {
                        toast.error(error.response.data.errorText);
                    }
                }
            ));

            commit('setUser', user);
        },
        async downloadCurrentUser({commit, state}) {
          const storedCurrentUser = sessionStorage.getItem(state.sessionStorageKeys.currentUserKey);

          if(!storedCurrentUser) {
            await axios.get(`${state.serverUrl}/useraccount/getcurrentuser`)
              .then(response => {
                if(response.status === 200 && response.data) {
                  const user = response.data;
                  commit('setCurrentUser', user);
                  commit('setRoles', user.roles)
                }
              })
              .catch(error => {
                  if(error.response) {
                      toast.error(error.response.data.errorText);
                  }
              });
          }
          else {
            commit('setCurrentUser', JSON.parse(storedCurrentUser));
            commit('setRoles', JSON.parse(storedCurrentUser).roles);
          }
        }
    }
});

export default store;
