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
          if(state.roles) {
            return state.roles.includes('Admin');
          }
          else {
            return false
          }
        },
        isOwner(state) {
          if(state.roles) {
            return state.roles.includes('Owner');
          }
          else {
            return false
          }
        },
        isUser(state) {
          if(state.roles) {
            return state.roles.includes('User');
          }
          else {
            return false
          }
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
          await axios.get(`${state.serverUrl}/chapters/get/${chapterId}`)
            .then(async response => {
              if(response.status === 200) {
                const chapter = response.data;
                commit('setChapter', chapter);
                commit('setThemes', chapter.themes);
              }}
            )
            .catch(error => {
              if(error.response) {
                toast.error(error.response.data.errorText);
              }
            });
        },
        async downloadChapters({dispatch, commit, state}) {
          const storedChapters = sessionStorage.getItem(state.sessionStorageKeys.chaptersKey);

          if(!storedChapters) {
            await axios.get(`${state.serverUrl}/chapters/getlist`)
              .then(async response => {
                 if(response.status === 200) {
                 const chapters = response.data;
                 commit('setChapters', chapters);
                 await dispatch('downloadChapterNodes');
                }}
              )
              .catch(error => {
                if(error.response) {
                  toast.error(error.response.data.errorText);
                }
              });
          }
          else {
            commit('setChapters', JSON.parse(storedChapters));
          }
        },
        async downloadChapterNodes({commit, state}) {
          const storedChapterNodes = sessionStorage.getItem(state.sessionStorageKeys.chapterNodesKey);

          if(!storedChapterNodes) {
            await axios.get(`${state.serverUrl}/chapters/getnodes`)
              .then(async response => {
                if(response.status === 200) {
                  commit('setChapterNodes', response.data);
                }
              })
              .catch(error => {
                if(error.response) {
                  toast.error(error.response.data.errorText);
                }
              });
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
              if(response.status === 200) {
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

          await axios.delete(`${state.serverUrl}/chapters/delete/` + chapter.chapterId, null)
            .then(async response => {
              if(response.status === 200) {
                toast.success('Раздел успешно удален');
                sessionStorage.removeItem(state.sessionStorageKeys.chaptersKey)
                sessionStorage.removeItem(state.sessionStorageKeys.chapterNodesKey)
                await dispatch('downloadChapters');
                router.push(`/`);
              }
            })
            .catch(error => {
              if(error.response) {
                toast.error(error.response.data.errorText);
              }
            });
        },
        async updateChapter({dispatch, state}, chapter) {
          await axios.put(`${state.serverUrl}/chapters/update`,  chapter, {
              headers: {
                  'Content': 'application/json',
                  'Accept': '*/*'
              }
            })
            .then(async response => {
              if(response.status === 200) {
                  toast.success('Раздел успешно обновлен');
                  sessionStorage.removeItem(state.sessionStorageKeys.chaptersKey);
                  sessionStorage.removeItem(state.sessionStorageKeys.chapterNodesKey);
                  await dispatch('downloadChapter',  chapter.chapterId);
                  await dispatch('downloadChapters');
                  router.push(`/chapters/${chapter.chapterId}${chapter.themes.length > 0 ? '/' + chapter.themes[0].themeId : '' }`);
              }
            })
            .catch(error => {
              if(error.response) {
                toast.error(error.response.data.errorText);
              }
            });
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
            await axios.get(`${state.serverUrl}/documents/getlist`)
              .then(response => {
                if(response.status === 200) {
                  commit('setDocuments', response.data);
                }
              })
              .catch(error => {
                if(error.response) {
                    toast.error(error.response.data.errorText)
                }
              });
          }
          else {
            commit('setDocuments', JSON.parse(storedDocuments));
          }
        },
        async downloadDocumentNodes({commit, state}) {
          const storedDocumentNodes = sessionStorage.getItem(state.sessionStorageKeys.documentNodesKey);

          if(!storedDocumentNodes) {
            await axios.get(`${state.serverUrl}/documents/getnodes`)
              .then(response => {
                if(response.status === 200) {
                  commit('setDocumentNodes', response.data);
                }
              })
              .catch(error => {
                if(error.response) {
                    toast.error(error.response.data.errorText)
                }
              });
          }
          else {
            commit('setDocumentNodes', JSON.parse(storedDocumentNodes));
          }
        },

        // MESSAGES
        async downloadMessages({commit, state}, isRead) {
          await axios.get(`${state.serverUrl}/feedback/getmessages/${isRead}`)
            .then(response => {
              if(response.status === 200) {
                commit('setMessages', response.data);
              }
            })
            .catch(error => {
              if(error.response) {
                  toast.error(error.response.data.errorText)
              }
            });
        },
        async downloadMessage({commit, state}, messageId) {
          await axios.get(`${state.serverUrl}/feedback/getmessage/${messageId}`)
            .then(response => {
              if(response.status === 200) {
                commit('setMessage', response.data);
              }
            })
            .catch(error => {
              if(error.response) {
                  toast.error(error.response.data.errorText)
              }
            });
        },
        async downloadUnreadMessagesCount({commit, state}) {
          await axios.get(`${state.serverUrl}/feedback/getunreadmessagescount`)
            .then(response => {
              if(response.status === 200) {
                commit('setUnreadMessagesCount', response.data);
              }
            });
        },

        // SEARCH
        async downloadSearchResult({commit, state}, searchLine) {
          await axios.post(`${state.serverUrl}/search/getsearchresult`, {
                 searchLine: searchLine
              }
            )
            .then(response => {
              if(response.status === 200) {
                commit('setSearchResult', response.data);
              }
            })
            .catch(error => {
              if(error.response) {
                  toast.error(error.response.data.errorText)
              }
            });
        },

        // CAPTCHA
        async downloadCaptcha({commit, state}) {
          await axios.get(`${state.serverUrl}/captcha/get`)
            .then(response => {
              if(response.status === 200) {
                commit('setCaptcha', response.data);
              }
            })
            .catch(error => {
              if(error.response) {
                toast.error(error.response.data.errorText);
              }
            });
        },

        // IMAGES
        async downloadImageNames({commit, state}) {
          await axios.get(`${state.serverUrl}/home/getimagenames`)
            .then(response => {
              if(response.status === 200) {
                commit('setImageNames', response.data);
              }
            })
            .catch(error => {
              if(error.response) {
                  toast.error(error.response.data.errorText);
              }
            });
        },

        // USERS
        async downloadUsers({commit, state}) {
          await axios.get(`${state.serverUrl}/administration/getusers`)
            .then(response => {
              if(response.status === 200) {
                commit('setUsers', response.data);
              }
            })
            .catch(error => {
              if(error.response) {
                  toast.error(error.response.data.errorText);
              }
            });
        },
        async downloadUser({commit, state}, userId) {
          await axios.get(`${state.serverUrl}/administration/getuser/${userId}`)
            .then(response => {
              if(response.status === 200) {
                commit('setUser', response.data);
              }
            })
            .catch(error => {
              if(error.response) {
                toast.error(error.response.data.errorText);
              }
            });
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
        },
    }
});

export default store;
