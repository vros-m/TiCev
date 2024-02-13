import { Link, Outlet, useLoaderData, useNavigate } from "react-router-dom";
import Button from '@mui/material/Button';
import { Dialog,DialogContent,IconButton,Avatar,Typography, Backdrop, CardContent, CardActionArea } from "@mui/material";
import LogoutIcon from '@mui/icons-material/Logout';
import AccountBoxIcon from '@mui/icons-material/AccountBox';
import NotificationsNoneIcon from '@mui/icons-material/NotificationsNone';
import NotificationsActiveIcon from '@mui/icons-material/NotificationsActive';
import CloseIcon from '@mui/icons-material/Close';
import SearchBar from "../GeneralComponents/Searchbar";
import { useContext, useEffect, useRef, useState } from "react";
import UserContext from "./Contexts/UserContext";
import AddCircleOutlineIcon from '@mui/icons-material/AddCircleOutline';
import DeleteIcon from '@mui/icons-material/Delete';
import { CURRENT_USER, formatRelativeTime, profilePicture, thumbnailByVideoId, userController } from "../Constants";

export async function NavbarLoader()
{
  const currentUser = JSON.parse(localStorage.getItem(CURRENT_USER))
  if (currentUser)
  {
    const response = await fetch(userController + '/GetNotifications/' + currentUser.id)
    if (response.ok)
    {
      const notifications = await response.json()
      return notifications
      }
  }
  return []
}
export default function Navbar()
{
  const [dialogOpen, setDialogOpen] = useState(false)
  const [userState, setUserState] = useContext(UserContext)
  const notifications=useLoaderData()
  const [unreadNotification,setUnreadNotification]=useState(false)

  function logout()
  {
    setUserState(null)
    navigate('/login')
  }
  const notificationsRef = useRef(null)
  const navigate = useNavigate()
  useEffect(() =>
  {
    if (userState == null) {
      navigate('/login')
    }
    else
    {
      if (notifications.length != 0)
        setUserState(oldValue => {
          return {
            ...oldValue,
            notifications:notifications
        }})
      const socket = new WebSocket('ws://localhost:5070/SocketService?userId=' + userState.id);
      socket.addEventListener('open', ev => console.log('SOCKET OPEN'))
      socket.addEventListener('message', msg => setUserState(oldValue => { 
        setUnreadNotification(true)
        const notification = JSON.parse(msg.data)
        const trueNotification = {}
        for (const key in notification)
        {
          const newKey = key.charAt(0).toLowerCase() + key.slice(1);
          trueNotification[newKey]=notification[key]
          }
        return {
          ...oldValue,
          notifications:[trueNotification,...oldValue.notifications]
        }
      }))
      socket.addEventListener('close', ev => console.log('SOCKET CLOSED'))

      }

  }, [])

  return <div id="root" >
          <div className="d-flex flex-row justify-content-between" style={{
      height: "100px",
      backgroundColor: "white",
          color: "black",
          position: 'fixed',
          top: '0px',
          padding: '10px',
          paddingLeft: '30px',
          zIndex: '1000',
          paddingRight: '20px',
              width:'100%'
      }}>
          <Link style={{ color: 'black', fontSize: "2.5rem" }} to="/">TiCev</Link>
          <SearchBar onSearch={(query) =>navigate('/'+query)} width={'500px'}/>
      <div className="d-flex flex-row align-items-center">
        <Button  sx={{ color: 'black' }} onClick={ev=>navigate('/postVideo')}>
          <AddCircleOutlineIcon sx={{ width: '35px', height: '35px' }}/>
        </Button>
              <Button ref={notificationsRef}
                  sx={{ color: 'black' }}
          onClick={(ev) =>
          {
            setUnreadNotification(false)
            setDialogOpen((oldValue=>!oldValue))
                  }}
        >{unreadNotification ?
            <NotificationsActiveIcon sx={{ width: '35px', height: '35px' }} /> :
            <NotificationsNoneIcon sx={{ width: '35px', height: '35px' }} />}
        </Button>
        <NotificationDialog isOpen={dialogOpen} onClose={ev => setDialogOpen(false)} anchorEl={notificationsRef}
          notifications={userState?.notifications?userState.notifications:[]} />
        <Button sx={{ color: 'black' }}><AccountBoxIcon sx={{ width: '35px', height: '35px' }}
          onClick={ev => navigate('profile')} /></Button>
              <Button sx={{color:'black'}} onClick={ev=>logout()}><LogoutIcon sx={{width:'35px',height:'35px'} }/></Button>
          </div>
      </div>
      <Outlet/>
  </div>

}

const NotificationCard = ({ notification, onCloseDialog }) => {
  
  const { senderId, message, videoId, recipientId, timestamp,id} = notification;
  const navigate = useNavigate()
  const [userState,setUserState] = useContext(UserContext)

  async function deleteNotification() {
    const request = await fetch(userController + '/DeleteNotification/' + id,
      { method: 'DELETE' })
    if (request.ok)
    {
      setUserState(oldValue => {
        return {
          ...oldValue,
          notifications:[...oldValue.notifications.filter(n=>n.id!=id)]
        }
      })
      }
  }
  function handleClick() { 
    onCloseDialog()
    navigate('/player/' + videoId)
  }
    return (
      <div
        style={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-around',
          padding: '8px',
          borderBottom: '1px solid #e0e0e0',
          minHeight: '80px',
                width: '100%',
          cursor:'pointer'
        }}
      ><CardActionArea
        sx={{ display: 'flex', justifyContent: 'space-between' }}>
          <div style={{
            display: 'flex', alignItems: 'center',
          }} onClick={ev =>handleClick()
            }>
          {/* User Avatar */}
          <Avatar src={profilePicture(senderId)} alt="User Avatar" style={{ marginRight: '12px', width: '60px', height: '60px' }} />
          {/* Content */}
          <div>
            <Typography variant="body1">{message}</Typography>
            <Typography variant="caption">{formatRelativeTime(timestamp)}</Typography>
          </div>
        </div>
        {/* Video Thumbnail */}
          <img src={thumbnailByVideoId(videoId)} alt="Video Thumbnail" style={{ height: '60px', width: 'auto' }} />  

      </CardActionArea>
      <Button sx={{color:'red'}} onClick={ev=>deleteNotification()}><DeleteIcon/></Button>  
      </div>
    );
  };
  
const NotificationDialog = ({ isOpen, onClose, anchorEl,notifications}) => {

/*     const [notifications, setNotifications] = useState([])
    useEffect(() => {
        const rawData = new Array(20).fill({
            userAvatar: "https://via.placeholder.com/30x30",
            content: "TESTING CONTENT TESTING NEW VIDEO OR NEW COMMENT OR NEW REPLY",
            timestamp:"2 minutes ago", videoThumbnail :'https://via.placeholder.com/345x200'
        })
        setNotifications(rawData)
    },[]) */
    return (
        <Dialog
            open={isOpen}
            onClose={onClose}
            maxWidth="md"
            fullWidth
            PaperProps={{
                style: {
                    position: 'absolute',
                    width: '650px',
                    top: anchorEl.current ? anchorEl.current.getBoundingClientRect().bottom : 0,
                    right: '50px',
                },
            }}
      >
        <DialogContent>
       
          {/* Notification Cards */}
          <div style={{ maxHeight: '400px', overflowY: 'auto' }}>
            {notifications.map((notification, index) => (
              <NotificationCard key={index} notification={notification} onCloseDialog={onClose} />
            ))}
          </div>
        </DialogContent>
      </Dialog>
    );
  };
  

