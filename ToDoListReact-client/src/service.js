import axios from 'axios';

const apiUrl = "http://localhost:5160/tasks";

axios.defaults.baseURL = apiUrl; // הגדרת כתובת ה-API כ-default

// הוספת interceptor שתופס את השגיאות ב-response ורושם ללוג
axios.interceptors.response.use(null, error => {
    console.error('Error encountered:', error);
    return Promise.reject(error);
});

export default {
  getTasks: async () => {
    const result = await axios.get(apiUrl);
    return result.data;
  },

  addTask: async(name)=>{
    console.log('addTask', name);
    //TODO
    const result = await axios.post("", { name: name }); // נשלחת השאילתא עם הנתון להוספת המשימה
    return result.data;
  },

  setCompleted: async(id, isComplete)=>{
    console.log('setCompleted', {id, isComplete});
    //TODO
    const result = await axios.put(`${apiUrl}/${id}`, { isComplete: isComplete }); // נשלחת השאילתא עם הנתון לעדכון סטטוס המשימה
    return result.data;
  },
  

  deleteTask: async(id)=>{
    console.log('deleteTask');
    //TODO
    const result = await axios.delete(`${apiUrl}/${id}`); // נשלחת השאילתא עם ה-ID של המשימה למחיקה
    return result.data;
  }
};
