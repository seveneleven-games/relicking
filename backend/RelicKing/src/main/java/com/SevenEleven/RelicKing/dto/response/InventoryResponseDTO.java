package com.SevenEleven.RelicKing.dto.response;

import java.util.List;

import com.SevenEleven.RelicKing.dto.model.MemberRelicDTO;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class InventoryResponseDTO {

	private int currentClassNo;

	private List<MemberRelicDTO> myRelicList;

}
