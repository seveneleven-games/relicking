package com.SevenEleven.RelicKing.dto.response;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.List;

import com.SevenEleven.RelicKing.dto.response.model.RelicResponseDTO;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class StageResponseDTO {

    private int currentClassNo;

    private List<RelicResponseDTO> relicList;

}
