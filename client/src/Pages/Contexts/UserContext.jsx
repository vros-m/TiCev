import { createContext } from "react";


const currentUser = localStorage.getItem('currentUser')
const UserContext = createContext(
    {
        userState: currentUser
        , setUserState:(oldValue)=>null
    }
)


export default UserContext;