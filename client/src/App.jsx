import { CURRENT_USER } from './Constants';
import UserContext from './Pages/Contexts/UserContext';
import { useState } from 'react';

export default function App({ children })
{
    const currentUser = JSON.parse(localStorage.getItem(CURRENT_USER))
    const [userState, setUserState1] = useState(currentUser)
    const setUserState = (newValue) =>
    {
        if (typeof newValue == 'function')
        {
            const newState = newValue(userState)
            localStorage.setItem(CURRENT_USER,JSON.stringify(newState))
        }
        else
        {
            localStorage.setItem(CURRENT_USER,JSON.stringify(newValue))
        }
        setUserState1(newValue)
    }

    return <UserContext.Provider value={
        [userState,setUserState]
    }>
        {children}
    </UserContext.Provider>
}
