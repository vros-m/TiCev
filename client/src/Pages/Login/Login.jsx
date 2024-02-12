import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import Alert from '@mui/material/Alert'
import './Login.css'
import { Link, useNavigate } from 'react-router-dom';
import { userController } from '../../Constants';
import { useContext, useState } from 'react';
import { loginUser } from './Signup.jsx';
import UserContext from '../Contexts/UserContext.jsx';



function loginFailed(setLoginState)
{
    setLoginState((oldValue) =>
    {
        setTimeout(setLoginState(oldValue),3000)
        return !oldValue
    })
}
export async function login(username, password,navigate,setLoginState,setUserState)
{
    try {
        const response = await fetch(`${userController}/Login/${username}/${password}`,{
    })
        if (response.status === 200)
        {
            const userState = await response.json() 
            loginUser(userState,setUserState,navigate)
            
        }
        else
        {
            loginFailed(setLoginState)
        }
    }
    catch (error)
    {
        console.log(error.message)
    }
   

}

export default function Login() { 

    const [userState,setUserState] = useContext(UserContext)
    const [loginFailedState,setLoginFailedState]=useState(false)
    const navigate = useNavigate()
    const [loginState,setLoginState] = useState({
        username:"",password:""
    })

    function onChange(event)
    {
        setLoginState((oldValue) =>
        {
            return {
                ...oldValue,
                [event.target.name]:event.target.value
            }
        })
    }

    return <div className="login_container">
        <h2>Log in</h2>
        <TextField id="Username" label="Username" variant="outlined" sx={{
            marginTop:"10px",
            marginBottom: "10px"
            
        }}
            value={loginState.username}
            name='username'
        onChange={(onChange)}/>
        <TextField id="Password" label="Password" variant="outlined" type='password'
            sx={{
                marginBottom:"10px"
            }}
            value={loginState.password}
            name='password'
        onChange={(onChange)}/>
        {loginFailedState && <p style={{
            color: 'red',
        }}>{'Login' + " Failed"}</p>}
        <Button variant="text" sx={{
            width:"80px"
        }} id='Button' onClick={(ev) =>login(loginState.username, loginState.password, navigate, setLoginFailedState,setUserState)
        }>OK</Button>
        <Link to={'/signup'}>{'Don\'t have an account? Sign up here.'}</Link>
       
    </div>
}

