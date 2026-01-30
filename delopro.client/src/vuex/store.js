import { createStore } from "vuex";
import axios from "axios";
import { useToast } from "vue-toastification";

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
        user: null
    },
    getters: {
        getChapter(state) {
            return state.chapter;
        },
        getChapters(state) {
            return state.chapters;
        },
        getChapterNodes(state) {
            return state.chapterNodes;
        },
        getTheme(state) {
            return state.theme;
        },
        getThemes(state) {
            return state.themes;
        },
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
        getMessages(state) {
            return state.messages;
        },
        getMessage(state) {
            return state.message;
        },
        getUnreadMessagesCount(state) {
            return state.unreadMessagesCount;
        },
        getSearchResult(state) {
            return state.searchResult;
        },
        serverUrl(state) {
            return state.serverUrl;
        },
        environment(state) {
            return state.environment;
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
        getUserRoles(state) {
            return state.roles;
        },
        getShowChapterList(state) {
            return state.showChapterList;
        },
        getShowRightColumn(state) {
            return state.showRightColumn;  
        },
        getCaptcha(state) {
            return state.captcha;
        },
        getPending(state) {
            return state.pending;
        },
        getImageNames(state) {
            return state.imageNames;
        },
        getUsers(state) {
            return state.users;
        },
        getUser(state) {
            return state.user;
        }
    },
    mutations: {
        setRoles(state, userRoles) {
            state.roles = userRoles;
        },
        setNickname(state, userNickname) {
            state.nickname = userNickname;
        },
        renderSearchBar(state) {
            state.title = null;
            state.showSearchBar = true;
        },
        setTitle(state, value) {
            state.title = value;
            state.showSearchBar = false;
        },
        setChapter(state, chapter) {            
            state.chapter = chapter;
        },
        setChapters(state, chapters) {
            state.chapters = chapters;
        },
        setChapterNodes(state, chapterNodes) {
            state.chapterNodes = chapterNodes;
        },
        setTheme(state, theme) {
            state.theme = theme;
        },
        setThemes(state, themes) {
            state.themes = themes;
        },
        setDocuments(state, documents) {
            state.documents = documents;
        },
        setDocumentNodes(state, documentNodes) {
            state.documentNodes = documentNodes;
        },
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
        setSearchResult(state, searchResult) {
            state.searchResult = searchResult;
        },
        setShowChapterList(state, value) {
            state.showChapterList = value;
        },
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
        },
        setUsers(state, value) {
            state.users = value;
        },
        setUser(state, value) {
            state.user = value;
        }
    },
    actions: {
        async downloadChapter({commit, state}, chapterId ) {
            const url = `${state.serverUrl}/chapters/get/${chapterId}`;
            const chapter = (await axios.get(url)).data;
            commit('setChapter', chapter);
            commit('setThemes', chapter.themes);
        },
        async downloadChapters({commit, state}) {
            const chapters = (await axios.get(`${state.serverUrl}/chapters/getlist`)).data;
            commit('setChapters', chapters);
        },
        async downloadChapterNodes({commit, state}) {
            const chapterNodes = (await axios.get(`${state.serverUrl}/chapters/getnodes`)).data;
            commit('setChapterNodes', chapterNodes);
        },
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

            //commit('setPending', true);
            
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
                //commit('setPending', false);
            }
        },
        async downloadDocuments({commit, state}) {
            const documents = (await axios.get(`${state.serverUrl}/documents/getlist`)
                .then(response => response.data)
                .catch(error => {
                    if(error.response) {
                        toast.error(error.response.data.errorText)
                    }
                }));

            commit('setDocuments', documents);
        },
        async downloadDocumentNodes({commit, state}) {
            const documentNodes = (await axios.get(`${state.serverUrl}/documents/getnodes`)
                .then(response => response.data)
                .catch(error => {
                    if(error.response) {
                        toast.error(error.response.data.errorText)
                    }
                }));

            commit('setDocumentNodes', documentNodes);
        },
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
                    if(response.status == 200) {
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
                    if(response.status == 200) {
                        return response.data;
                    }
                });

            commit('setUnreadMessagesCount', count);
        },
        async downloadSearchResult({commit, state}, searchLine) {
            const searchResult = (await axios.post(`${state.serverUrl}/search/getsearchresult`,
                {
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
        }
    }
});

export default store;