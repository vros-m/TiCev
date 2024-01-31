import SubmitField from "./SubmitField";
import SearchIcon from '@mui/icons-material/Search';

export default function SearchBar({onSearch,width})
{
    return <SubmitField
        onSubmit={onSearch}
        icon={<SearchIcon/>}
        label="Search..."
        width={width}
    />
}